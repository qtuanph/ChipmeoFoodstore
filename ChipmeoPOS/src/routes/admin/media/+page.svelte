<script lang="ts">
	import { onMount } from 'svelte';
	import { formatDate } from '$lib/utils/index.js';
	import Button from '$lib/components/Button.svelte';
	import Toast from '$lib/components/Toast.svelte';
	import { MediaState } from './media.svelte.js';

	const vm = new MediaState();
	let fileInput: HTMLInputElement | null = $state(null);

	onMount(async () => {
		vm.init();
	});

	async function handleDelete(id: number) {
		if (!confirm('Bạn có chắc muốn xóa file này không?')) return;
		await vm.handleDelete(id);
	}

	async function handleUpload(event: Event) {
		const input = event.target as HTMLInputElement;
		if (!input.files || input.files.length === 0) return;
		await vm.handleUpload(input.files[0]);
		if (fileInput) fileInput.value = '';
	}
</script>

<svelte:head>
	<title>Quản lý Media - Admin</title>
</svelte:head>

<div class="p-6">
	<div class="mb-6 flex items-center justify-between">
		<div>
			<h1 class="text-2xl font-bold text-gray-900">Quản lý Media</h1>
			<p class="text-gray-600">Thư viện ảnh và tệp tin ({vm.filteredFiles.length} files)</p>
		</div>
		<div class="flex gap-2">
			<input type="file" class="hidden" bind:this={fileInput} onchange={handleUpload} />
			<Button variant="primary" onclick={() => fileInput?.click()} disabled={vm.uploading}>
				{vm.uploading ? 'Đang tải lên...' : 'Upload File'}
			</Button>
		</div>
	</div>

	<!-- Folder Tabs -->
	<div class="mb-6 flex flex-wrap gap-2 border-b pb-2">
		{#each vm.folders as folder (folder)}
			<button
				class="rounded-lg px-4 py-2 text-sm font-medium transition-colors {vm.selectedFolder ===
				folder
					? 'bg-indigo-100 text-indigo-700'
					: 'text-gray-600 hover:bg-gray-100'}"
				onclick={() => (vm.selectedFolder = folder)}
			>
				{folder === 'All' ? 'Tất cả' : folder}
			</button>
		{/each}
	</div>

	{#if vm.loading}
		<div class="flex justify-center py-12">
			<div class="h-10 w-10 animate-spin rounded-full border-b-2 border-indigo-600"></div>
		</div>
	{:else if vm.filteredFiles.length === 0}
		<div class="rounded-lg border-2 border-dashed border-gray-200 bg-gray-50 py-12 text-center">
			<p class="text-gray-500">Không có file nào trong thư mục này.</p>
		</div>
	{:else}
		<div class="grid grid-cols-2 gap-4 md:grid-cols-4 lg:grid-cols-6">
			{#each vm.filteredFiles as file (file.id)}
				<div
					class="group relative overflow-hidden rounded-lg border bg-white shadow-sm transition-shadow hover:shadow-md"
				>
					<!-- Image Preview -->
					<div class="relative aspect-square overflow-hidden bg-gray-100">
						{#if file.fileType?.startsWith('image/')}
							<img
								src={file.fileUrl}
								alt={file.fileName}
								class="h-full w-full object-cover transition-transform group-hover:scale-105"
								loading="lazy"
							/>
						{:else}
							<div class="flex h-full w-full items-center justify-center text-4xl text-gray-400">
								📄
							</div>
						{/if}

						<!-- Overlay Actions -->
						<div
							class="absolute inset-0 flex items-center justify-center gap-2 bg-black/50 opacity-0 transition-opacity group-hover:opacity-100"
						>
							<a
								data-sveltekit-reload
								href={file.fileUrl}
								target="_blank"
								rel="noopener noreferrer"
								class="rounded-full bg-white p-2 text-gray-700 hover:text-indigo-600"
								title="Xem"
							>
								👁️
							</a>
							<button
								class="rounded-full bg-white p-2 text-red-600 hover:bg-red-50"
								onclick={() => handleDelete(file.id)}
								title="Xóa"
							>
								🗑️
							</button>
						</div>
					</div>

					<!-- Meta Info -->
					<div class="p-2 text-xs">
						<p class="truncate font-medium" title={file.fileName}>{file.fileName}</p>
						<div class="mt-1 flex justify-between text-gray-500">
							<span>{Math.round((file.fileSize || 0) / 1024)} KB</span>
							<span>{formatDate(file.createdAt)}</span>
						</div>
						{#if file.entityType}
							<p
								class="mt-1 inline-block rounded bg-indigo-50 px-1 py-0.5 text-[10px] text-indigo-600"
							>
								🔗 {file.entityType} #{file.entityId}
							</p>
						{/if}
					</div>
				</div>
			{/each}
		</div>
	{/if}
</div>

<Toast bind:show={vm.showToast} message={vm.toastMessage} type={vm.toastType} />
