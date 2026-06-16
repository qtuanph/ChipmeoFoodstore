<script lang="ts">
	import { onMount, onDestroy } from 'svelte';
	import { Editor } from '@tiptap/core';
	import StarterKit from '@tiptap/starter-kit';
	import { CustomImage } from '$lib/utils/custom-image.js';
	import Link from '@tiptap/extension-link';
	import Placeholder from '@tiptap/extension-placeholder';
	import TextAlign from '@tiptap/extension-text-align';
	import Underline from '@tiptap/extension-underline';
	import FloatingMenu from '@tiptap/extension-floating-menu';
	import MediaLibraryModal from '$lib/components/media/MediaLibraryModal.svelte';
	import LinkModal from './LinkModal.svelte';
	import { BubbleMenu } from '@tiptap/extension-bubble-menu';
	import ImageSettingsModal from './ImageSettingsModal.svelte';

	interface Props {
		content?: string;
		placeholder?: string;
		onchange?: (html: string) => void;
	}

	let { content = '', placeholder = 'Nhấn "/" để mở menu lệnh...', onchange }: Props = $props();

	let element: HTMLDivElement;
	let editor: Editor | null = $state(null);
	let showMediaLibrary = $state(false);
	let floatingMenuElement: HTMLDivElement;
	let bubbleMenuElement: HTMLDivElement;
	let fileInput: HTMLInputElement;

	// Link Modal State
	let showLinkModal = $state(false);
	let linkInitialUrl = $state('');
	let linkInitialText = $state('');

	// Image Settings Modal State
	let showImageSettingsModal = $state(false);
	let imageSettingsData = $state({ href: '', caption: '' });
	let imageSettingsPos: number | null = null;

	function handleOpenImageSettings(e: CustomEvent) {
		const { pos, href, caption } = e.detail;
		imageSettingsPos = pos;
		imageSettingsData = { href, caption };
		showImageSettingsModal = true;
	}

	function handleSaveImageSettings(href: string, caption: string) {
		if (editor && imageSettingsPos !== null) {
			// Update node attributes at specific position
			// Reuse existing attrs to prevent overwrite
			const node = editor.state.doc.nodeAt(imageSettingsPos);
			if (node) {
				const { tr } = editor.state;
				const transaction = tr.setNodeMarkup(imageSettingsPos, undefined, {
					...node.attrs,
					href,
					caption
				});
				editor.view.dispatch(transaction);
			}
		}
	}

	// Floating Menu State
	let isMenuOpen = $state(false);

	function runMenuAction(action: () => void) {
		action();
		isMenuOpen = false;
	}

	// Store pending files: Blob URL -> File
	let pendingFiles = new Map<string, File>();

	export async function uploadImages(uploadFn: (file: File) => Promise<string>): Promise<string> {
		if (!editor) return content;

		const currentHtml = editor.getHTML();
		const parser = new DOMParser();
		const doc = parser.parseFromString(currentHtml, 'text/html');
		const images = doc.querySelectorAll('img');

		const uploadPromises: Promise<void>[] = [];

		for (const img of images) {
			const src = img.getAttribute('src');
			if (src && src.startsWith('blob:') && pendingFiles.has(src)) {
				const file = pendingFiles.get(src);
				if (file) {
					const promise = uploadFn(file).then((newUrl) => {
						img.setAttribute('src', newUrl);
					});
					uploadPromises.push(promise);
				}
			}
		}

		if (uploadPromises.length > 0) {
			await Promise.all(uploadPromises);
			const newContent = doc.body.innerHTML;
			editor.commands.setContent(newContent);
			return newContent;
		}

		return currentHtml;
	}

	onMount(() => {
		editor = new Editor({
			element,
			extensions: [
				StarterKit.configure({
					heading: { levels: [2, 3, 4] },
					paragraph: {
						HTMLAttributes: { class: 'editor-paragraph' }
					}
				}),
				CustomImage,
				Link.configure({
					openOnClick: false,
					HTMLAttributes: {
						class: 'text-indigo-600 underline cursor-pointer hover:text-indigo-800'
					}
				}),
				Placeholder.configure({ placeholder }),
				TextAlign.configure({ types: ['heading', 'paragraph'] }),
				Underline,
				FloatingMenu.configure({
					element: floatingMenuElement,
					tippyOptions: { duration: 100, placement: 'left', offset: [0, 24], maxWidth: 'none' }
				} as any),
				BubbleMenu.configure({
					element: bubbleMenuElement,
					tippyOptions: { duration: 100, placement: 'bottom' },
					shouldShow: ({ editor }: { editor: Editor }) => {
						return editor.isActive('customImage');
					}
				} as any)
			],
			content,
			onUpdate: ({ editor }) => {
				onchange?.(editor.getHTML());
			},
			editorProps: {
				attributes: {
					class: 'prose prose-lg max-w-3xl mx-auto focus:outline-none min-h-[400px] py-8'
				}
			}
		});

		// Listen for custom event from CustomImage NodeView
		element.addEventListener('open-image-settings', handleOpenImageSettings as EventListener);
	});

	onDestroy(() => {
		editor?.destroy();
		for (const url of pendingFiles.keys()) {
			URL.revokeObjectURL(url);
		}
	});

	function handleLibrarySelect(url: string) {
		if (url && editor) {
			editor
				.chain()
				.focus()
				.setImage({ src: url, alt: '', align: 'center' } as any)
				.run();
		}
		showMediaLibrary = false;
	}

	function handleFileUpload(event: Event) {
		const input = event.target as HTMLInputElement;
		const file = input.files?.[0];
		if (file && editor) {
			const blobUrl = URL.createObjectURL(file);
			pendingFiles.set(blobUrl, file);
			editor
				.chain()
				.focus()
				.setImage({ src: blobUrl, alt: file.name, align: 'center' } as any)
				.run();
		}
		input.value = '';
	}

	function openLinkModal() {
		if (!editor) return;
		const previousUrl = editor.getAttributes('link').href || '';
		linkInitialUrl = previousUrl;
		const { from, to } = editor.state.selection;
		const text = editor.state.doc.textBetween(from, to, ' ');
		linkInitialText = text;

		showLinkModal = true;
	}

	function handleLinkSave(url: string, text?: string) {
		if (!editor) return;

		if (url === '') {
			editor.chain().focus().extendMarkRange('link').unsetLink().run();
			return;
		}

		if (text && text !== linkInitialText) {
			editor
				.chain()
				.focus()
				.extendMarkRange('link')
				.setLink({ href: url })
				.command(({ tr, dispatch }) => {
					if (dispatch) {
						tr.insertText(text);
					}
					return true;
				})
				.run();
			editor.chain().focus().extendMarkRange('link').setLink({ href: url }).run();
		} else {
			editor.chain().focus().extendMarkRange('link').setLink({ href: url }).run();
		}
	}
</script>

<div class="group relative">
	<!-- Bubble Menu for Image (Hidden unless active) -->
	<div
		bind:this={bubbleMenuElement}
		class="flex items-center gap-1 rounded-lg border border-gray-200 bg-white p-1 shadow-lg"
		style="visibility: hidden;"
	>
		{#if editor?.isActive('customImage')}
			<button
				type="button"
				onclick={() =>
					editor?.chain().focus().updateAttributes('customImage', { align: 'left' }).run()}
				class="rounded p-1.5 hover:bg-gray-100 {editor?.getAttributes('customImage').align ===
				'left'
					? 'bg-indigo-50 text-indigo-600'
					: 'text-gray-600'}"
				title="Căn trái"
			>
				<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
					><path
						stroke-linecap="round"
						stroke-linejoin="round"
						stroke-width="2"
						d="M4 6h16M4 12h16M4 18h16"
					/></svg
				>
			</button>
			<button
				type="button"
				onclick={() =>
					editor?.chain().focus().updateAttributes('customImage', { align: 'center' }).run()}
				class="rounded p-1.5 hover:bg-gray-100 {editor?.getAttributes('customImage').align ===
				'center'
					? 'bg-indigo-50 text-indigo-600'
					: 'text-gray-600'}"
				title="Căn giữa"
			>
				<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
					><path
						stroke-linecap="round"
						stroke-linejoin="round"
						stroke-width="2"
						d="M4 6h16M4 12h16M4 18h16"
					/></svg
				>
			</button>
			<button
				type="button"
				onclick={() =>
					editor?.chain().focus().updateAttributes('customImage', { align: 'right' }).run()}
				class="rounded p-1.5 hover:bg-gray-100 {editor?.getAttributes('customImage').align ===
				'right'
					? 'bg-indigo-50 text-indigo-600'
					: 'text-gray-600'}"
				title="Căn phải"
			>
				<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
					><path
						stroke-linecap="round"
						stroke-linejoin="round"
						stroke-width="2"
						d="M4 6h16M4 12h16M4 18h16"
					/></svg
				>
			</button>
		{/if}
	</div>

	<!-- Main Editor Area -->
	<div
		class="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm transition-all focus-within:border-indigo-500 focus-within:ring-2 focus-within:ring-indigo-500/20"
	>
		<!-- Minimal Top Toolbar -->
		<div
			class="relative z-10 flex flex-wrap items-center gap-1 border-b border-gray-100 bg-gray-50/50 p-2"
		>
			<button
				type="button"
				onclick={() => editor?.chain().focus().toggleBold().run()}
				class="rounded-lg p-1.5 text-gray-500 transition-all hover:bg-white hover:text-gray-900 hover:shadow-sm {editor?.isActive(
					'bold'
				)
					? 'bg-white text-indigo-600 shadow-sm'
					: ''}"
				title="Bold (Ctrl+B)"
			>
				<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
					><path
						stroke-linecap="round"
						stroke-linejoin="round"
						stroke-width="2"
						d="M6 4h8a4 4 0 014 4 4 4 0 01-4 4H6V4zm0 8h9a4 4 0 014 4 4 4 0 01-4 4H6v-8z"
					/></svg
				>
			</button>
			<button
				type="button"
				onclick={() => editor?.chain().focus().toggleItalic().run()}
				class="rounded-lg p-1.5 text-gray-500 transition-all hover:bg-white hover:text-gray-900 hover:shadow-sm {editor?.isActive(
					'italic'
				)
					? 'bg-white text-indigo-600 shadow-sm'
					: ''}"
				title="Italic (Ctrl+I)"
			>
				<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
					><path
						stroke-linecap="round"
						stroke-linejoin="round"
						stroke-width="2"
						d="M10 20h4M10 4h4M16 4l-8 16"
					/></svg
				>
			</button>
			<button
				type="button"
				onclick={() => editor?.chain().focus().toggleUnderline().run()}
				class="rounded-lg p-1.5 text-gray-500 transition-all hover:bg-white hover:text-gray-900 hover:shadow-sm {editor?.isActive(
					'underline'
				)
					? 'bg-white text-indigo-600 shadow-sm'
					: ''}"
				title="Underline (Ctrl+U)"
			>
				<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
					><path
						stroke-linecap="round"
						stroke-linejoin="round"
						stroke-width="2"
						d="M6 3v10a6 6 0 0012 0V3"
					/></svg
				>
			</button>
			<div class="mx-1 h-4 w-px bg-gray-300"></div>
			<!-- Text Alignment -->
			<button
				type="button"
				onclick={() => editor?.chain().focus().setTextAlign('left').run()}
				class="rounded-lg p-1.5 text-gray-500 transition-all hover:bg-white hover:text-gray-900 hover:shadow-sm {editor?.isActive(
					{ textAlign: 'left' }
				)
					? 'bg-white text-indigo-600 shadow-sm'
					: ''}"
				title="Căn trái"
			>
				<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
					><path
						stroke-linecap="round"
						stroke-linejoin="round"
						stroke-width="2"
						d="M4 6h16M4 12h10M4 18h16"
					/></svg
				>
			</button>
			<button
				type="button"
				onclick={() => editor?.chain().focus().setTextAlign('center').run()}
				class="rounded-lg p-1.5 text-gray-500 transition-all hover:bg-white hover:text-gray-900 hover:shadow-sm {editor?.isActive(
					{ textAlign: 'center' }
				)
					? 'bg-white text-indigo-600 shadow-sm'
					: ''}"
				title="Căn giữa"
			>
				<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
					><path
						stroke-linecap="round"
						stroke-linejoin="round"
						stroke-width="2"
						d="M4 6h16M7 12h10M4 18h16"
					/></svg
				>
			</button>
			<button
				type="button"
				onclick={() => editor?.chain().focus().setTextAlign('right').run()}
				class="rounded-lg p-1.5 text-gray-500 transition-all hover:bg-white hover:text-gray-900 hover:shadow-sm {editor?.isActive(
					{ textAlign: 'right' }
				)
					? 'bg-white text-indigo-600 shadow-sm'
					: ''}"
				title="Căn phải"
			>
				<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
					><path
						stroke-linecap="round"
						stroke-linejoin="round"
						stroke-width="2"
						d="M4 6h16M10 12h10M4 18h16"
					/></svg
				>
			</button>

			<!-- Image Alignment (Contextual) -->
			{#if editor?.isActive('customImage')}
				<div class="mx-1 h-4 w-px bg-gray-300"></div>
				<span class="px-1 text-xs font-medium text-indigo-500">Img:</span>
				<button
					type="button"
					onclick={() =>
						editor?.chain().focus().updateAttributes('customImage', { align: 'left' }).run()}
					class="rounded-lg p-1.5 text-gray-500 transition-all hover:bg-white hover:text-gray-900 hover:shadow-sm {editor?.getAttributes(
						'customImage'
					).align === 'left'
						? 'bg-white text-indigo-600 shadow-sm'
						: ''}"
					title="Ảnh trái"
				>
					<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
						><path
							stroke-linecap="round"
							stroke-linejoin="round"
							stroke-width="2"
							d="M4 6h16M4 12h16M4 18h16"
						/></svg
					>
					<!-- TODO: Better icon -->
				</button>
				<button
					type="button"
					onclick={() =>
						editor?.chain().focus().updateAttributes('customImage', { align: 'center' }).run()}
					class="rounded-lg p-1.5 text-gray-500 transition-all hover:bg-white hover:text-gray-900 hover:shadow-sm {editor?.getAttributes(
						'customImage'
					).align === 'center'
						? 'bg-white text-indigo-600 shadow-sm'
						: ''}"
					title="Ảnh giữa"
				>
					<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
						><path
							stroke-linecap="round"
							stroke-linejoin="round"
							stroke-width="2"
							d="M4 6h16M4 12h16M4 18h16"
						/></svg
					>
				</button>
				<button
					type="button"
					onclick={() =>
						editor?.chain().focus().updateAttributes('customImage', { align: 'right' }).run()}
					class="rounded-lg p-1.5 text-gray-500 transition-all hover:bg-white hover:text-gray-900 hover:shadow-sm {editor?.getAttributes(
						'customImage'
					).align === 'right'
						? 'bg-white text-indigo-600 shadow-sm'
						: ''}"
					title="Ảnh phải"
				>
					<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
						><path
							stroke-linecap="round"
							stroke-linejoin="round"
							stroke-width="2"
							d="M4 6h16M4 12h16M4 18h16"
						/></svg
					>
				</button>
			{/if}

			<div class="mx-1 h-4 w-px bg-gray-300"></div>
			<button
				type="button"
				onclick={openLinkModal}
				class="rounded-lg p-1.5 text-gray-500 transition-all hover:bg-white hover:text-gray-900 hover:shadow-sm {editor?.isActive(
					'link'
				)
					? 'bg-white text-indigo-600 shadow-sm'
					: ''}"
				title="Chèn Link"
			>
				<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
					><path
						stroke-linecap="round"
						stroke-linejoin="round"
						stroke-width="2"
						d="M13.828 10.172a4 4 0 00-5.656 0l-4 4a4 4 0 105.656 5.656l1.102-1.101m-.758-4.899a4 4 0 005.656 0l4-4a4 4 0 00-5.656-5.656l-1.1 1.1"
					/></svg
				>
			</button>
			<button
				type="button"
				onclick={() => fileInput.click()}
				class="rounded-lg p-1.5 text-gray-500 transition-all hover:bg-white hover:text-gray-900 hover:shadow-sm"
				title="Tải ảnh lên"
			>
				<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
					><path
						stroke-linecap="round"
						stroke-linejoin="round"
						stroke-width="2"
						d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"
					/></svg
				>
			</button>
		</div>

		<!-- Editor Content -->
		<div bind:this={element} class="rich-editor min-h-[500px]"></div>
	</div>

	<!-- Floating Menu (Gutter) -->
	<div bind:this={floatingMenuElement} class="flex items-center" style="visibility: hidden;">
		<div class="relative flex items-center">
			<!-- The + Button -->
			<button
				type="button"
				onclick={() => (isMenuOpen = !isMenuOpen)}
				class="flex h-6 w-6 items-center justify-center rounded text-gray-400 transition-colors hover:bg-gray-100 hover:text-gray-900 {isMenuOpen
					? 'bg-gray-100 text-gray-900'
					: ''}"
				title="Thêm nội dung"
			>
				<svg
					class="h-5 w-5 transition-transform duration-200 {isMenuOpen ? 'rotate-45' : ''}"
					fill="none"
					stroke="currentColor"
					viewBox="0 0 24 24"
					><path
						stroke-linecap="round"
						stroke-linejoin="round"
						stroke-width="2"
						d="M12 4v16m8-8H4"
					/></svg
				>
			</button>

			<!-- The Menu Items -->
			{#if isMenuOpen}
				<div
					class="absolute left-full ml-2 flex items-center gap-1 rounded-lg border border-gray-100 bg-white p-1 shadow-xl duration-200 animate-in fade-in slide-in-from-left-2"
				>
					<button
						type="button"
						onclick={() =>
							runMenuAction(() => editor?.chain().focus().toggleHeading({ level: 2 }).run())}
						class="min-w-[32px] rounded p-1.5 text-xs font-bold text-gray-600 hover:bg-gray-100"
						title="Heading 2"
					>
						H2
					</button>
					<button
						type="button"
						onclick={() =>
							runMenuAction(() => editor?.chain().focus().toggleHeading({ level: 3 }).run())}
						class="min-w-[32px] rounded p-1.5 text-xs font-bold text-gray-600 hover:bg-gray-100"
						title="Heading 3"
					>
						H3
					</button>
					<div class="mx-1 h-4 w-px bg-gray-200"></div>
					<button
						type="button"
						onclick={() => runMenuAction(() => editor?.chain().focus().toggleBulletList().run())}
						class="rounded p-1.5 text-gray-600 hover:bg-gray-100"
						title="Danh sách"
					>
						<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
							><path
								stroke-linecap="round"
								stroke-linejoin="round"
								stroke-width="2"
								d="M4 6h16M4 12h16M4 18h16M4 6a1 1 0 11-2 0 1 1 0 012 0zm0 6a1 1 0 11-2 0 1 1 0 012 0zm0 6a1 1 0 11-2 0 1 1 0 012 0z"
							/></svg
						>
					</button>
					<button
						type="button"
						onclick={() => runMenuAction(() => editor?.chain().focus().toggleOrderedList().run())}
						class="rounded p-1.5 text-gray-600 hover:bg-gray-100"
						title="Danh sách số"
					>
						<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
							><path
								stroke-linecap="round"
								stroke-linejoin="round"
								stroke-width="2"
								d="M7 7h12M7 13h12M7 19h12M5 6a1 1 0 11-2 0 1 1 0 012 0zm0 7a1 1 0 11-2 0 1 1 0 012 0zm0 6a1 1 0 11-2 0 1 1 0 012 0z"
							/></svg
						>
					</button>
					<div class="mx-1 h-4 w-px bg-gray-200"></div>
					<button
						type="button"
						onclick={() => runMenuAction(() => fileInput.click())}
						class="rounded p-1.5 text-gray-600 hover:bg-gray-100"
						title="Tải ảnh lên (Mới)"
					>
						<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
							><path
								stroke-linecap="round"
								stroke-linejoin="round"
								stroke-width="2"
								d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-8l-4-4m0 0L8 8m4-4v12"
							/></svg
						>
					</button>
					<button
						type="button"
						onclick={() => runMenuAction(() => (showMediaLibrary = true))}
						class="rounded p-1.5 text-gray-600 hover:bg-gray-100"
						title="Chọn từ thư viện"
					>
						<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
							><path
								stroke-linecap="round"
								stroke-linejoin="round"
								stroke-width="2"
								d="M4 16l4.586-4.586a2 2 0 012.828 0L16 16m-2-2l1.586-1.586a2 2 0 012.828 0L20 14m-6-6h.01M6 20h12a2 2 0 002-2V6a2 2 0 00-2-2H6a2 2 0 00-2 2v12a2 2 0 002 2z"
							/></svg
						>
					</button>
					<div class="mx-1 h-4 w-px bg-gray-200"></div>
					<button
						type="button"
						onclick={() => runMenuAction(() => editor?.chain().focus().toggleCodeBlock().run())}
						class="rounded p-1.5 text-gray-600 hover:bg-gray-100"
						title="Code Block"
					>
						<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
							><path
								stroke-linecap="round"
								stroke-linejoin="round"
								stroke-width="2"
								d="M10 20l4-16m4 4l4 4-4 4M6 16l-4-4 4-4"
							/></svg
						>
					</button>
					<button
						type="button"
						onclick={() => runMenuAction(() => editor?.chain().focus().toggleBlockquote().run())}
						class="rounded p-1.5 text-gray-600 hover:bg-gray-100"
						title="Trích dẫn"
					>
						<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
							><path
								stroke-linecap="round"
								stroke-linejoin="round"
								stroke-width="2"
								d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"
							/></svg
						>
					</button>
					<button
						type="button"
						onclick={() => runMenuAction(() => editor?.chain().focus().setHorizontalRule().run())}
						class="rounded p-1.5 text-gray-600 hover:bg-gray-100"
						title="Đường kẻ"
					>
						<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"
							><path
								stroke-linecap="round"
								stroke-linejoin="round"
								stroke-width="2"
								d="M20 12H4"
							/></svg
						>
					</button>
				</div>
			{/if}
		</div>
	</div>
</div>

<!-- Hidden File Input -->
<input
	type="file"
	accept="image/*"
	class="hidden"
	bind:this={fileInput}
	onchange={handleFileUpload}
/>

<!-- Media Library Modal -->
<MediaLibraryModal
	bind:open={showMediaLibrary}
	folder="blog"
	lockFolder={true}
	onSelect={handleLibrarySelect}
	onClose={() => (showMediaLibrary = false)}
/>

<LinkModal
	bind:open={showLinkModal}
	initialUrl={linkInitialUrl}
	initialText={linkInitialText}
	onSave={handleLinkSave}
	onClose={() => (showLinkModal = false)}
/>

<ImageSettingsModal
	bind:open={showImageSettingsModal}
	initialLink={imageSettingsData.href}
	initialCaption={imageSettingsData.caption}
	onSave={handleSaveImageSettings}
	onClose={() => (showImageSettingsModal = false)}
/>

<style>
	/* Editor container */
	:global(.rich-editor .ProseMirror) {
		min-height: 500px;
		padding: 1.5rem;
		outline: none;
		font-size: 16px;
		line-height: 1.75;
		color: #374151;
	}

	/* Tighter paragraph spacing */
	:global(.rich-editor .ProseMirror p) {
		margin: 0.5em 0;
	}

	/* Placeholder with Notion-like look */
	:global(.rich-editor .ProseMirror p.is-editor-empty:first-child::before) {
		content: attr(data-placeholder);
		float: left;
		color: #9ca3af;
		pointer-events: none;
		height: 0;
		font-style: italic;
	}

	/* Headings */
	:global(.rich-editor .ProseMirror h2) {
		font-size: 1.8em;
		font-weight: 700;
		margin: 2em 0 0.5em;
		color: #111827;
		letter-spacing: -0.025em;
	}

	:global(.rich-editor .ProseMirror h3) {
		font-size: 1.5em;
		font-weight: 600;
		margin: 1.5em 0 0.5em;
		color: #1f2937;
	}

	/* Lists */
	:global(.rich-editor .ProseMirror ul),
	:global(.rich-editor .ProseMirror ol) {
		padding-left: 1.5rem;
		margin: 0.5em 0;
	}

	:global(.rich-editor .ProseMirror ul) {
		list-style-type: disc;
	}
	:global(.rich-editor .ProseMirror ol) {
		list-style-type: decimal;
	}

	/* Blockquote */
	:global(.rich-editor .ProseMirror blockquote) {
		border-left: 4px solid #e5e7eb;
		padding-left: 1rem;
		margin: 1.5em 0;
		color: #4b5563;
		font-style: italic;
	}

	/* Code */
	:global(.rich-editor .ProseMirror pre) {
		background: #1f2937;
		color: #e5e7eb;
		padding: 1rem;
		border-radius: 0.5rem;
		overflow-x: auto;
		font-family: monospace;
		margin: 1.5em 0;
	}

	/* Images */
	:global(.rich-editor .ProseMirror img) {
		max-width: 100%;
		height: auto;
		border-radius: 0.75rem;
		margin: 2em 0;
		box-shadow:
			0 4px 6px -1px rgba(0, 0, 0, 0.1),
			0 2px 4px -1px rgba(0, 0, 0, 0.06);
	}

	/* Image selected state */
	:global(.rich-editor .ProseMirror img.ProseMirror-selectednode) {
		outline: 2px solid #6366f1;
	}
</style>
