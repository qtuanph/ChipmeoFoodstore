<script lang="ts">
	import { onMount, untrack } from 'svelte';
	import {
		getMediaFiles,
		getFolders,
		uploadMedia,
		deleteMedia,
		formatFileSize,
		type MediaFile
	} from '$lib/api/mediaStorage.ts';

	interface Props {
		open: boolean;
		folder?: string;
		lockFolder?: boolean;
		onSelect: (url: string) => void;
		onClose: () => void;
	}

	let {
		open = $bindable(false),
		folder = 'blog',
		lockFolder = false,
		onSelect,
		onClose
	}: Props = $props();

	let mediaFiles: MediaFile[] = $state([]);
	let folders: string[] = $state([]);
	let selectedFolder = $state(untrack(() => folder));

	$effect(() => {
		if (folder) selectedFolder = folder;
	});
	let selectedFile: MediaFile | null = $state(null);
	let loading = $state(false);
	let uploading = $state(false);
	let searchTerm = $state('');
	let fileInput: HTMLInputElement | null = $state(null);
	let dragOver = $state(false);
	let dialogElement: HTMLDialogElement | null = $state(null);

	onMount(async () => {
		await Promise.all([loadFolders(), loadMedia()]);
	});

	$effect(() => {
		if (open && dialogElement) {
			dialogElement.showModal();
		} else if (!open && dialogElement) {
			dialogElement.close();
		}
	});

	async function loadFolders() {
		try {
			folders = await getFolders();
		} catch (err) {
			console.error('Failed to load folders:', err);
		}
	}

	async function loadMedia() {
		loading = true;
		try {
			const folderFilter = selectedFolder === 'all' ? undefined : selectedFolder;
			mediaFiles = await getMediaFiles(folderFilter);
		} catch (err) {
			console.error('Failed to load media:', err);
		} finally {
			loading = false;
		}
	}

	async function handleUpload(files: FileList | null) {
		if (!files || files.length === 0) return;

		uploading = true;
		try {
			const targetFolder = selectedFolder === 'all' ? folder || 'blog' : selectedFolder;

			for (const file of files) {
				const result = await uploadMedia(file, targetFolder);
				mediaFiles = [result, ...mediaFiles];
			}
		} catch (err: unknown) {
			const message = err instanceof Error ? err.message : 'Upload thất bại!';
			alert(message);
		} finally {
			uploading = false;
		}
	}

	async function handleDelete() {
		if (!selectedFile) return;
		if (!confirm(`Xóa ảnh "${selectedFile.fileName}"?`)) return;

		try {
			const filename = selectedFile.fileUrl.split('/').pop() || '';
			await deleteMedia(selectedFile.folder, filename);
			mediaFiles = mediaFiles.filter((f) => f.fileUrl !== selectedFile!.fileUrl);
			selectedFile = null;
		} catch (err: unknown) {
			const message = err instanceof Error ? err.message : 'Xóa thất bại!';
			alert(message);
		}
	}

	function handleInsert() {
		if (selectedFile) {
			onSelect(selectedFile.fileUrl);
			open = false;
		}
	}

	function handleDrop(e: DragEvent) {
		e.preventDefault();
		dragOver = false;
		handleUpload(e.dataTransfer?.files ?? null);
	}

	function handleFileSelect(e: Event) {
		const input = e.target as HTMLInputElement;
		handleUpload(input.files);
		input.value = '';
	}

	function handleBackdropClick(e: MouseEvent) {
		if (e.target === dialogElement) {
			onClose();
		}
	}

	const filteredMedia = $derived(
		mediaFiles.filter((f) => f.fileName.toLowerCase().includes(searchTerm.toLowerCase()))
	);

	function formatDate(dateString: string) {
		return new Date(dateString).toLocaleDateString('vi-VN', {
			day: '2-digit',
			month: '2-digit',
			year: 'numeric',
			hour: '2-digit',
			minute: '2-digit'
		});
	}

	$effect(() => {
		if (selectedFolder) {
			loadMedia();
		}
	});
</script>

<dialog
	bind:this={dialogElement}
	class="fixed inset-0 m-auto h-[85vh] w-full max-w-6xl overflow-hidden rounded-xl bg-white p-0 shadow-2xl backdrop:bg-black/60"
	aria-label="Thư viện Media"
	onclick={handleBackdropClick}
>
	<div class="flex h-full w-full flex-col">
		<!-- Header -->
		<div
			class="flex items-center justify-between gap-4 border-b border-gray-200 bg-gray-50 px-5 py-4"
		>
			<h2 class="truncate text-xl font-bold text-gray-900">📷 Thư viện Media</h2>
			<div class="flex shrink-0 items-center gap-3">
				<button
					type="button"
					onclick={() => fileInput?.click()}
					disabled={uploading}
					class="flex items-center gap-2 rounded-lg bg-amber-600 px-4 py-2 text-white transition-colors hover:bg-amber-700 disabled:opacity-50"
				>
					{#if uploading}
						<svg class="h-4 w-4 animate-spin" fill="none" viewBox="0 0 24 24">
							<circle
								class="opacity-25"
								cx="12"
								cy="12"
								r="10"
								stroke="currentColor"
								stroke-width="4"
							></circle>
							<path
								class="opacity-75"
								fill="currentColor"
								d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"
							></path>
						</svg>
						Đang tải...
					{:else}
						📤 Tải lên ảnh mới
					{/if}
				</button>
				<button
					type="button"
					onclick={() => onClose()}
					class="rounded-lg p-2 text-gray-500 hover:bg-gray-200"
					title="Đóng"
					aria-label="Đóng"
				>
					✕
				</button>
			</div>
			<input
				type="file"
				accept="image/*"
				multiple
				bind:this={fileInput}
				onchange={handleFileSelect}
				class="hidden"
				aria-hidden="true"
			/>
		</div>

		<div class="flex flex-1 overflow-hidden">
			<!-- Sidebar - only show when not locked -->
			{#if !lockFolder}
				<div class="w-48 space-y-4 overflow-y-auto border-r border-gray-200 bg-gray-50 p-4">
					<div>
						<h3 class="mb-2 text-xs font-semibold text-gray-500 uppercase">Thư mục</h3>
						<div class="space-y-1">
							{#each folders as f (f)}
								<button
									type="button"
									onclick={() => (selectedFolder = f)}
									class="w-full rounded-lg px-3 py-2 text-left text-sm transition-colors {selectedFolder ===
									f
										? 'bg-amber-600 text-white'
										: 'text-gray-700 hover:bg-gray-200'}"
								>
									📂 {f}
								</button>
							{/each}
						</div>
					</div>
				</div>
			{/if}

			<!-- Main Content -->
			<div class="flex flex-1 flex-col overflow-hidden">
				<!-- Search -->
				<div class="border-b border-gray-200 p-4">
					<div class="relative">
						<svg
							class="absolute top-1/2 left-3 h-5 w-5 -translate-y-1/2 text-gray-400"
							fill="none"
							stroke="currentColor"
							viewBox="0 0 24 24"
						>
							<path
								stroke-linecap="round"
								stroke-linejoin="round"
								stroke-width="2"
								d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"
							></path>
						</svg>
						<input
							type="text"
							id="media-search"
							bind:value={searchTerm}
							placeholder="Tìm kiếm ảnh..."
							class="w-full max-w-md rounded-lg border border-gray-300 py-2 pr-4 pl-10 focus:border-transparent focus:ring-2 focus:ring-amber-500"
						/>
					</div>
				</div>

				<!-- Grid -->
				<div
					class="flex-1 overflow-y-auto p-4 {dragOver ? 'bg-amber-50' : ''}"
					role="region"
					aria-label="Khu vực kéo thả file"
					ondragover={(e) => {
						e.preventDefault();
						dragOver = true;
					}}
					ondragleave={() => (dragOver = false)}
					ondrop={handleDrop}
				>
					{#if loading}
						<div class="flex h-full items-center justify-center">
							<svg class="h-10 w-10 animate-spin text-amber-600" fill="none" viewBox="0 0 24 24">
								<circle
									class="opacity-25"
									cx="12"
									cy="12"
									r="10"
									stroke="currentColor"
									stroke-width="4"
								></circle>
								<path
									class="opacity-75"
									fill="currentColor"
									d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"
								></path>
							</svg>
						</div>
					{:else if filteredMedia.length === 0}
						<div class="flex h-full flex-col items-center justify-center text-gray-500">
							<div class="mb-4 text-6xl">🖼️</div>
							<p class="text-lg">Chưa có ảnh nào</p>
							<p class="mt-2 text-sm">Kéo thả hoặc nhấn "Tải lên ảnh mới" để bắt đầu</p>
						</div>
					{:else}
						<div
							class="grid grid-cols-3 gap-3 sm:grid-cols-4 md:grid-cols-5 lg:grid-cols-6"
							role="listbox"
							aria-label="Danh sách ảnh"
						>
							{#each filteredMedia as file (file.fileUrl)}
								<button
									type="button"
									role="option"
									aria-selected={selectedFile?.fileUrl === file.fileUrl}
									class="group relative aspect-square cursor-pointer overflow-hidden rounded-lg border-2 text-left transition-all hover:shadow-lg {selectedFile?.fileUrl ===
									file.fileUrl
										? 'border-amber-600 ring-2 ring-amber-300'
										: 'border-gray-200'}"
									onclick={() => (selectedFile = file)}
								>
									<img
										src={file.fileUrl}
										alt={file.fileName}
										class="h-full w-full object-cover"
										loading="lazy"
									/>
									{#if selectedFile?.fileUrl === file.fileUrl}
										<div
											class="absolute top-2 right-2 flex h-6 w-6 items-center justify-center rounded-full bg-amber-600 text-sm text-white"
										>
											✓
										</div>
									{/if}
									<div
										class="absolute right-0 bottom-0 left-0 bg-gradient-to-t from-black/70 to-transparent p-2 opacity-0 transition-opacity group-hover:opacity-100"
									>
										<p class="truncate text-xs text-white">{file.fileName}</p>
									</div>
								</button>
							{/each}
						</div>
					{/if}
				</div>
			</div>

			<!-- Details Panel -->
			{#if selectedFile}
				<div class="w-72 overflow-y-auto border-l border-gray-200 bg-gray-50 p-4">
					<div class="space-y-4">
						<div class="aspect-video overflow-hidden rounded-lg bg-gray-200">
							<img
								src={selectedFile.fileUrl}
								alt={selectedFile.fileName}
								class="h-full w-full object-contain"
							/>
						</div>

						<dl class="space-y-2 text-sm">
							<div>
								<dt class="text-gray-500">Tên file:</dt>
								<dd class="font-medium break-all text-gray-900">{selectedFile.fileName}</dd>
							</div>
							<div>
								<dt class="text-gray-500">Thư mục:</dt>
								<dd class="font-medium text-gray-900">{selectedFile.folder}</dd>
							</div>
							<div>
								<dt class="text-gray-500">Kích thước:</dt>
								<dd class="font-medium text-gray-900">{formatFileSize(selectedFile.fileSize)}</dd>
							</div>
							<div>
								<dt class="text-gray-500">Ngày tạo:</dt>
								<dd class="font-medium text-gray-900">{formatDate(selectedFile.createdAt)}</dd>
							</div>
						</dl>

						<div class="space-y-2 border-t border-gray-200 pt-4">
							<button
								type="button"
								onclick={handleInsert}
								class="w-full rounded-lg bg-amber-600 px-4 py-2.5 font-medium text-white transition-colors hover:bg-amber-700"
							>
								✓ Chèn ảnh này
							</button>
							<button
								type="button"
								onclick={() => navigator.clipboard.writeText(selectedFile!.fileUrl)}
								class="w-full rounded-lg bg-gray-200 px-4 py-2 text-sm text-gray-700 transition-colors hover:bg-gray-300"
							>
								📋 Copy URL
							</button>
							<button
								type="button"
								onclick={handleDelete}
								class="w-full rounded-lg px-4 py-2 text-sm text-red-600 transition-colors hover:bg-red-50"
							>
								🗑️ Xóa ảnh
							</button>
						</div>
					</div>
				</div>
			{/if}
		</div>

		<!-- Footer -->
		<div class="flex items-center justify-between border-t border-gray-200 bg-gray-50 px-5 py-3">
			<span class="text-sm text-gray-500">
				{filteredMedia.length} ảnh {selectedFolder !== 'all' ? `trong "${selectedFolder}"` : ''}
			</span>
			<div class="flex items-center gap-3">
				<button
					type="button"
					onclick={() => onClose()}
					class="px-4 py-2 text-gray-600 hover:text-gray-900"
				>
					Hủy
				</button>
				<button
					type="button"
					onclick={handleInsert}
					disabled={!selectedFile}
					class="rounded-lg bg-amber-600 px-6 py-2 font-medium text-white transition-colors hover:bg-amber-700 disabled:cursor-not-allowed disabled:opacity-50"
				>
					Chèn ảnh
				</button>
			</div>
		</div>
	</div>
</dialog>

<style>
	dialog::backdrop {
		background: rgba(0, 0, 0, 0.6);
	}

	dialog[open] {
		display: flex;
	}
</style>
