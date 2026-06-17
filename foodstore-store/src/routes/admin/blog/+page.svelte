<script lang="ts">
	import { onMount } from 'svelte';
	import type { BlogPost } from '$lib/api/blog.js';
	import RichTextEditor from '$lib/components/editor/RichTextEditor.svelte';
	import SEOPanel from '$lib/components/editor/SEOPanel.svelte';
	import MediaLibraryModal from '$lib/components/media/MediaLibraryModal.svelte';
	import { BlogState } from './blog.svelte.js';
	import Icon from '$lib/components/ui/Icon.svelte';

	const blog = new BlogState();

	// UI Local State
	let thumbnailInput: HTMLInputElement | null = $state(null);
	let editorComponent: any = $state();
	let showThumbnailPicker = $state(false);

	onMount(() => {
		blog.init();
	});

	async function onThumbnailSelect(event: Event) {
		const input = event.target as HTMLInputElement;
		const file = input.files?.[0];
		if (!file) return;
		try {
			await blog.uploadThumbnail(file);
		} catch (err) {
			alert('Lỗi khi tải ảnh lên!');
		} finally {
			input.value = '';
		}
	}

	async function onSave() {
		try {
			await blog.handleSave(editorComponent);
		} catch (err: any) {
			alert(err.message || 'Lỗi khi lưu bài viết');
		}
	}

	async function onDelete(post: BlogPost) {
		if (!confirm(`Bạn có chắc muốn xóa bài viết "${post.title}"?`)) return;
		try {
			await blog.handleDelete(post);
		} catch (err: any) {
			alert(err.message || 'Lỗi khi xóa bài viết');
		}
	}
</script>

<div class="space-y-6 p-6">
	<!-- Header -->
	<div class="flex items-center justify-between">
		<div>
			<h1 class="text-2xl font-bold text-gray-900">Quản lý Blog & SEO</h1>
			<p class="text-gray-500">Tạo và tối ưu bài viết cho SEO</p>
		</div>
		<button
			onclick={() => blog.openNewPost()}
			class="flex items-center gap-2 rounded-lg bg-amber-600 px-4 py-2 text-white transition-colors hover:bg-amber-700"
		>
			<Icon name="tabler:plus" class="h-5 w-5" />
			Tạo bài viết
		</button>
	</div>

	<!-- Posts Table -->
	<div class="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
		{#if blog.loading}
			<div class="p-8 text-center">
				<svg
					class="mx-auto h-8 w-8 animate-spin text-amber-600"
					xmlns="http://www.w3.org/2000/svg"
					fill="none"
					viewBox="0 0 24 24"
				>
					<circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"
					></circle>
					<path
						class="opacity-75"
						fill="currentColor"
						d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"
					></path>
				</svg>
			</div>
		{:else if blog.posts.length === 0}
			<div class="p-8 text-center text-gray-500">
				<div class="mb-2 text-4xl">📝</div>
				<p>Chưa có bài viết nào</p>
			</div>
		{:else}
			<table class="min-w-full divide-y divide-gray-200">
				<thead class="bg-gray-50">
					<tr>
						<th
							class="px-6 py-3 text-left text-xs font-medium tracking-wider text-gray-500 uppercase"
							>Tiêu đề</th
						>
						<th
							class="px-6 py-3 text-left text-xs font-medium tracking-wider text-gray-500 uppercase"
							>SEO</th
						>
						<th
							class="px-6 py-3 text-left text-xs font-medium tracking-wider text-gray-500 uppercase"
							>Trạng thái</th
						>
						<th
							class="px-6 py-3 text-left text-xs font-medium tracking-wider text-gray-500 uppercase"
							>Ngày tạo</th
						>
						<th
							class="px-6 py-3 text-right text-xs font-medium tracking-wider text-gray-500 uppercase"
							>Thao tác</th
						>
					</tr>
				</thead>
				<tbody class="divide-y divide-gray-200 bg-white">
					{#each blog.posts as post (post.id || post.slug)}
						<tr class="hover:bg-gray-50">
							<td class="px-6 py-4">
								<div class="flex items-center gap-3">
									{#if post.thumbnailUrl}
										<img src={post.thumbnailUrl} alt="" class="h-12 w-12 rounded-lg object-cover" />
									{:else}
										<div
											class="flex h-12 w-12 items-center justify-center rounded-lg bg-gray-100 text-2xl"
										>
											📄
										</div>
									{/if}
									<div>
										<div class="font-medium text-gray-900">{post.title}</div>
										<div class="text-sm text-gray-500">{post.slug}</div>
									</div>
								</div>
							</td>
							<td class="px-6 py-4">
								<div class="flex items-center gap-2">
									<span class="text-lg font-semibold {blog.getSeoScoreColor(post.seoScore)}"
										>{post.seoScore || 0}</span
									>
									<span class="text-xs text-gray-400">/100</span>
								</div>
								{#if post.focusKeyword}
									<div class="mt-1 text-xs text-gray-500">🎯 {post.focusKeyword}</div>
								{/if}
							</td>
							<td class="px-6 py-4">
								<span
									class="rounded-full px-2 py-1 text-xs font-medium {blog.getStatusBadge(
										post.status || 'draft'
									)}"
								>
									{blog.getStatusText(post.status || 'draft')}
								</span>
							</td>
							<td class="px-6 py-4 text-sm text-gray-500">
								{blog.formatDate(post.createdAt)}
							</td>
							<td class="px-6 py-4 text-right">
								<button
									onclick={() => blog.openEditPost(post)}
									class="mr-3 text-amber-600 hover:text-amber-900"
								>
									Sửa
								</button>
								<button onclick={() => onDelete(post)} class="text-red-600 hover:text-red-900">
									Xóa
								</button>
							</td>
						</tr>
					{/each}
				</tbody>
			</table>
		{/if}
	</div>
</div>

<!-- Editor Modal (Full Screen) -->
{#if blog.showModal}
	<div class="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
		<div class="flex max-h-[95vh] w-full max-w-7xl flex-col rounded-xl bg-gray-100">
			<!-- Modal Header -->
			<div
				class="flex items-center justify-between rounded-t-xl border-b border-gray-200 bg-white p-4"
			>
				<h2 class="text-xl font-bold text-gray-900">
					{blog.editingPost ? 'Chỉnh sửa bài viết' : 'Tạo bài viết mới'}
				</h2>
				<div class="flex items-center gap-3">
					<select
						bind:value={blog.formData.status}
						class="rounded-lg border border-gray-300 px-3 py-2 focus:ring-2 focus:ring-amber-500"
					>
						<option value="draft">Nháp</option>
						<option value="published">Xuất bản</option>
						<option value="archived">Lưu trữ</option>
					</select>
					<button
						onclick={() => (blog.showPreview = true)}
						class="mr-2 rounded-lg bg-gray-100 px-4 py-2 font-medium text-gray-700 hover:bg-gray-200"
					>
						👁️ Xem trước
					</button>
					<button
						onclick={onSave}
						disabled={blog.saving || !blog.formData.title}
						class="flex items-center gap-2 rounded-lg bg-amber-600 px-6 py-2 text-white hover:bg-amber-700 disabled:cursor-not-allowed disabled:opacity-50"
					>
						{#if blog.saving}
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
						{/if}
						{blog.saving ? 'Đang lưu...' : 'Lưu bài viết'}
					</button>
					<button
						type="button"
						onclick={() => (blog.showModal = false)}
						class="p-2 text-gray-500 hover:text-gray-700"
						title="Đóng"
						aria-label="Đóng"
					>
						<Icon name="tabler:x" class="h-6 w-6" />
					</button>
				</div>
			</div>

			<!-- Modal Content (New Layout) -->
			<div class="flex flex-1 overflow-hidden">
				<!-- 1. Main Editor Area -->
				<div class="flex-1 overflow-y-auto p-8 md:pr-12">
					<div class="mx-auto max-w-4xl space-y-6">
						<!-- Title -->
						<div>
							<input
								type="text"
								bind:value={blog.formData.title}
								placeholder="Tiêu đề bài viết..."
								class="w-full border-none bg-transparent p-0 text-4xl font-bold placeholder-gray-300 focus:ring-0"
							/>
						</div>

						<!-- Excerpt -->
						<div>
							<textarea
								bind:value={blog.formData.excerpt}
								placeholder="Thêm tóm tắt ngắn cho bài viết..."
								rows="2"
								class="w-full resize-none border-none bg-transparent p-0 text-lg text-gray-500 italic placeholder-gray-300 focus:ring-0"
							></textarea>
						</div>

						<!-- Rich Text Editor -->
						<div class="min-h-[500px]">
							<RichTextEditor
								bind:this={editorComponent}
								content={blog.formData.content}
								onchange={(html) => (blog.formData.content = html)}
								placeholder="Nội dung bài viết..."
							/>
						</div>
					</div>
				</div>

				<!-- 2. Sidebar Content Panel -->
				<div class="flex w-80 flex-col overflow-y-auto border-l border-gray-200 bg-white">
					<div class="border-b border-gray-100 p-4">
						<h3 class="text-xs font-bold tracking-wider text-gray-900 uppercase">
							{#if blog.activeSidebarTab === 'info'}
								THÔNG TIN CƠ BẢN
							{:else if blog.activeSidebarTab === 'seo'}
								TỐI ƯU SEO
							{:else if blog.activeSidebarTab === 'settings'}
								CÀI ĐẶT KHÁC
							{:else if blog.activeSidebarTab === 'analytics'}
								PHÂN TÍCH SEO
							{/if}
						</h3>
					</div>

					<div class="space-y-6 p-4">
						{#if blog.activeSidebarTab === 'info'}
							<!-- Thumbnail -->
							<div class="space-y-2">
								<label for="post-thumbnail" class="text-sm font-medium text-gray-700"
									>Ảnh đại diện</label
								>
								<div
									id="post-thumbnail"
									class="group relative aspect-video overflow-hidden rounded-lg border border-gray-200 bg-gray-100 transition-colors hover:border-amber-500"
								>
									{#if blog.formData.thumbnailUrl}
										<img
											src={blog.formData.thumbnailUrl}
											alt="Thumbnail"
											class="h-full w-full object-cover"
										/>
										<button
											class="absolute top-2 right-2 rounded-full bg-white/90 p-1 text-red-500 opacity-0 transition-opacity group-hover:opacity-100 hover:text-red-700"
											onclick={() => (blog.formData.thumbnailUrl = '')}
											title="Xóa ảnh"
										>
											<Icon name="tabler:x" class="h-4 w-4" />
										</button>
									{:else}
										<div
											class="flex h-full w-full flex-col items-center justify-center text-gray-400"
										>
											<Icon name="tabler:photo" class="mb-2 h-8 w-8" />
											<span class="text-xs">Chưa có ảnh</span>
										</div>
									{/if}

									<div
										class="absolute inset-0 flex items-center justify-center bg-black/0 transition-colors group-hover:bg-black/5"
									>
										{#if !blog.formData.thumbnailUrl}
											<button
												onclick={() => (showThumbnailPicker = true)}
												class="rounded bg-white px-3 py-1.5 text-xs font-medium text-gray-700 shadow-sm hover:shadow"
											>
												Chọn ảnh
											</button>
										{/if}
									</div>
								</div>
								<div class="flex gap-2">
									<button
										type="button"
										onclick={() => (showThumbnailPicker = true)}
										class="flex-1 rounded border border-gray-200 bg-gray-50 px-3 py-2 text-xs font-medium text-gray-700 hover:bg-gray-100"
									>
										Thư viện
									</button>
									<button
										type="button"
										onclick={() => thumbnailInput?.click()}
										class="flex-1 rounded border border-gray-200 bg-gray-50 px-3 py-2 text-xs font-medium text-gray-700 hover:bg-gray-100"
									>
										Tải lên
									</button>
								</div>
								<input
									type="file"
									accept="image/*"
									bind:this={thumbnailInput}
									onchange={onThumbnailSelect}
									class="hidden"
								/>
							</div>

							<!-- Status -->
							<div class="space-y-2">
								<label for="post-status" class="text-sm font-medium text-gray-700">Trạng thái</label
								>
								<select
									id="post-status"
									bind:value={blog.formData.status}
									class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:ring-2 focus:ring-amber-500"
								>
									<option value="draft">Nháp</option>
									<option value="published">Đã xuất bản</option>
									<option value="archived">Lưu trữ</option>
								</select>
							</div>

							<!-- Tags Selector -->
							<div class="space-y-2">
								<label for="post-tags" class="text-sm font-medium text-gray-700">Thẻ (Tags)</label>
								<div
									id="post-tags"
									class="max-h-40 overflow-y-auto rounded-lg border border-gray-300 bg-gray-50 p-2"
								>
									{#each blog.availableTags as tag (tag.id)}
										<label
											class="flex cursor-pointer items-center gap-2 rounded p-1 hover:bg-white"
										>
											<input
												type="checkbox"
												checked={blog.formData.tagIds.includes(tag.id)}
												onchange={() => blog.toggleTag(tag.id)}
												class="rounded text-amber-500 focus:ring-amber-500"
											/>
											<span
												class="rounded-full px-2 py-0.5 text-xs text-white"
												style="background-color: {tag.color}"
											>
												{tag.name}
											</span>
										</label>
									{/each}
									{#if blog.availableTags.length === 0}
										<p class="py-2 text-center text-xs text-gray-500">Chưa có thẻ nào</p>
									{/if}
								</div>
							</div>
						{:else if blog.activeSidebarTab === 'seo'}
							<!-- Focus Keyword -->
							<div class="space-y-2">
								<label for="post-focus-keyword" class="text-sm font-medium text-gray-700"
									>Từ khóa chính</label
								>
								<input
									id="post-focus-keyword"
									type="text"
									bind:value={blog.formData.focusKeyword}
									placeholder="Ví dụ: ẩm thực hà nội"
									class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:ring-2 focus:ring-amber-500"
								/>
							</div>

							<!-- Meta Title -->
							<div class="space-y-2">
								<div class="flex justify-between">
									<label for="post-meta-title" class="text-sm font-medium text-gray-700"
										>Meta Title</label
									>
									<span
										class="text-xs {(blog.formData.metaTitle || blog.formData.title).length > 60
											? 'text-red-500'
											: 'text-gray-400'}"
									>
										{(blog.formData.metaTitle || blog.formData.title).length}/60
									</span>
								</div>
								<input
									id="post-meta-title"
									type="text"
									bind:value={blog.formData.metaTitle}
									placeholder={blog.formData.title}
									class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:ring-2 focus:ring-amber-500"
								/>
							</div>

							<!-- Meta Description -->
							<div class="space-y-2">
								<div class="flex justify-between">
									<label for="post-meta-description" class="text-sm font-medium text-gray-700"
										>Meta Description</label
									>
									<span class="text-xs text-gray-400">
										{blog.formData.metaDescription.length}/160
									</span>
								</div>
								<textarea
									id="post-meta-description"
									bind:value={blog.formData.metaDescription}
									rows="4"
									placeholder="Mô tả ngắn gọn..."
									class="w-full resize-none rounded-lg border border-gray-300 px-3 py-2 text-sm focus:ring-2 focus:ring-amber-500"
								></textarea>
							</div>

							<!-- Preview Google -->
							<div class="border-t border-gray-100 pt-4">
								<p class="mb-2 text-xs font-medium text-gray-500 uppercase">Xem trước Google</p>
								<div class="overflow-hidden rounded bg-gray-50 p-3 text-sm">
									<div
										class="cursor-pointer truncate text-lg leading-snug font-medium text-[#1a0dab] hover:underline"
									>
										{blog.formData.metaTitle || blog.formData.title || 'Tiêu đề bài viết'}
									</div>
									<div class="text-xs text-[#006621]">foodstore.local › blog › ...</div>
									<div class="mt-1 line-clamp-2 text-xs text-[#545454]">
										{blog.formData.metaDescription ||
											blog.formData.excerpt ||
											'Mô tả bài viết sẽ hiển thị ở đây...'}
									</div>
								</div>
							</div>
						{:else if blog.activeSidebarTab === 'settings'}
							<!-- Other Keywords -->
							<div class="space-y-2">
								<label for="post-other-keywords" class="text-sm font-medium text-gray-700"
									>Từ khóa phụ</label
								>
								<input
									id="post-other-keywords"
									type="text"
									bind:value={blog.formData.keywords}
									placeholder="ngon, bổ, rẻ..."
									class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:ring-2 focus:ring-amber-500"
								/>
							</div>

							<!-- Canonical -->
							<div class="space-y-2">
								<label for="post-canonical-url" class="text-sm font-medium text-gray-700"
									>Canonical URL</label
								>
								<input
									id="post-canonical-url"
									type="text"
									bind:value={blog.formData.canonicalUrl}
									placeholder="https://..."
									class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:ring-2 focus:ring-amber-500"
								/>
							</div>

							<!-- OG Image -->
							<div class="space-y-2">
								<label for="post-og-image" class="text-sm font-medium text-gray-700"
									>Open Graph Image</label
								>
								<input
									id="post-og-image"
									type="text"
									bind:value={blog.formData.ogImageUrl}
									placeholder="URL ảnh chia sẻ XH..."
									class="w-full rounded-lg border border-gray-300 px-3 py-2 text-sm focus:ring-2 focus:ring-amber-500"
								/>
							</div>
						{:else if blog.activeSidebarTab === 'analytics'}
							<SEOPanel
								title={blog.formData.title}
								content={blog.formData.content}
								excerpt={blog.formData.excerpt}
								focusKeyword={blog.formData.focusKeyword}
								metaTitle={blog.formData.metaTitle}
								metaDescription={blog.formData.metaDescription}
								onScoreChange={(score) => (blog.formData.seoScore = score)}
							/>
						{/if}
					</div>
				</div>

				<!-- 3. Icon Strip (Navigation) -->
				<div class="flex w-14 flex-col items-center gap-2 border-l border-gray-200 bg-gray-50 py-4">
					<button
						onclick={() => (blog.activeSidebarTab = 'info')}
						class="tool-btn rounded-lg p-2 transition-all {blog.activeSidebarTab === 'info'
							? 'bg-white text-amber-600 shadow-md ring-1 ring-black/5'
							: 'text-gray-400 hover:bg-white/50 hover:text-gray-600'}"
						title="Thông tin cơ bản"
					>
						<Icon name="tabler:info-circle" class="h-5 w-5" />
					</button>
					<button
						onclick={() => (blog.activeSidebarTab = 'seo')}
						class="tool-btn rounded-lg p-2 transition-all {blog.activeSidebarTab === 'seo'
							? 'bg-white text-amber-600 shadow-md ring-1 ring-black/5'
							: 'text-gray-400 hover:bg-white/50 hover:text-gray-600'}"
						title="Cấu hình SEO"
					>
						<Icon name="tabler:search" class="h-5 w-5" />
					</button>
					<button
						onclick={() => (blog.activeSidebarTab = 'analytics')}
						class="tool-btn rounded-lg p-2 transition-all {blog.activeSidebarTab === 'analytics'
							? 'bg-white text-green-600 shadow-md ring-1 ring-black/5'
							: 'text-gray-400 hover:bg-white/50 hover:text-gray-600'}"
						title="Phân tích & Điểm số"
					>
						<div class="relative">
							<Icon name="tabler:chart-bar" class="h-5 w-5" />
							{#if blog.formData.seoScore && blog.formData.seoScore >= 80}
								<span class="absolute -top-1 -right-1 h-2 w-2 rounded-full bg-green-500"></span>
							{/if}
						</div>
					</button>
					<div class="my-1 h-px w-8 bg-gray-200"></div>
					<button
						onclick={() => (blog.activeSidebarTab = 'settings')}
						class="tool-btn rounded-lg p-2 transition-all {blog.activeSidebarTab === 'settings'
							? 'bg-white text-amber-600 shadow-md ring-1 ring-black/5'
							: 'text-gray-400 hover:bg-white/50 hover:text-gray-600'}"
						title="Cài đặt nâng cao"
					>
						<Icon name="tabler:settings" class="h-5 w-5" />
					</button>
				</div>
			</div>
		</div>
	</div>
{/if}

<!-- Thumbnail Picker Modal -->
<MediaLibraryModal
	bind:open={showThumbnailPicker}
	folder="blog"
	lockFolder={true}
	onSelect={(url) => {
		blog.formData.thumbnailUrl = url;
		showThumbnailPicker = false;
	}}
	onClose={() => (showThumbnailPicker = false)}
/>

<!-- Preview Modal Overlay -->
{#if blog.showPreview}
	<div
		class="fixed inset-0 z-[60] flex items-center justify-center bg-black/50 p-4 backdrop-blur-sm"
		onclick={() => (blog.showPreview = false)}
		role="button"
		tabindex="0"
		onkeydown={(e) => e.key === 'Escape' && (blog.showPreview = false)}
	>
		<!-- svelte-ignore a11y_click_events_have_key_events -->
		<!-- svelte-ignore a11y_no_noninteractive_element_interactions -->
		<div
			class="flex h-[90vh] w-full max-w-5xl flex-col overflow-hidden rounded-xl bg-white shadow-2xl"
			onclick={(e) => e.stopPropagation()}
			onkeydown={(e) => e.stopPropagation()}
			role="document"
			tabindex="-1"
		>
			<div class="flex items-center justify-between border-b border-gray-100 bg-gray-50 p-4">
				<h3 class="font-bold text-gray-800">👁️ Xem trước bài viết</h3>
				<button
					onclick={() => (blog.showPreview = false)}
					class="text-2xl text-gray-500 hover:text-gray-700">&times;</button
				>
			</div>
			<div class="flex-1 overflow-y-auto bg-white p-8">
				<!-- Preview Content -->
				<article class="mx-auto prose prose-lg max-w-4xl">
					{#if blog.formData.thumbnailUrl}
						<img
							src={blog.formData.thumbnailUrl}
							alt={blog.formData.title}
							class="mb-8 h-96 w-full rounded-2xl object-cover shadow-lg"
						/>
					{/if}

					<div class="mb-4 flex flex-wrap gap-2">
						{#each blog.availableTags.filter( (t) => blog.formData.tagIds.includes(t.id) ) as tag (tag.id)}
							<span
								class="rounded-full px-3 py-1 text-sm font-medium text-white shadow-sm"
								style="background-color: {tag.color}"
							>
								{tag.name}
							</span>
						{/each}
					</div>

					<h1 class="mb-6 text-4xl leading-tight font-extrabold text-gray-900 md:text-5xl">
						{blog.formData.title || 'Tiêu đề bài viết'}
					</h1>

					{#if blog.formData.excerpt}
						<p class="mb-8 border-l-4 border-amber-500 pl-4 text-xl text-gray-500 italic">
							{blog.formData.excerpt}
						</p>
					{/if}

					<div class="mt-8">
						{@html blog.formData.content}
					</div>
				</article>
			</div>
		</div>
	</div>
{/if}

<style>
	/* Preview Styles */
	:global(.prose figure) {
		margin: 2em 0;
	}
	:global(.prose figcaption) {
		text-align: center;
		color: #6b7280;
		font-size: 0.875em;
		margin-top: 0.5em;
		font-style: italic;
	}
</style>
