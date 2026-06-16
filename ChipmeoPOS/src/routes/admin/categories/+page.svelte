<script lang="ts">
	import { onMount } from 'svelte';
	import Button from '$lib/components/Button.svelte';
	import Card from '$lib/components/Card.svelte';
	import Modal from '$lib/components/Modal.svelte';
	import Toast from '$lib/components/Toast.svelte';
	import ImageCropper from '$lib/components/ImageCropper.svelte';
	import { CategoriesState } from './categories.svelte.js';

	const vm = new CategoriesState();
	let imageInput: HTMLInputElement | null = $state(null);

	onMount(() => {
		vm.init();
	});

	function handleImageSelect(event: Event) {
		const input = event.target as HTMLInputElement;
		const file = input.files?.[0];
		if (!file) return;
		vm.handleImageSelect(file);
		input.value = '';
	}
</script>

<svelte:head>
	<title>Quản lý Danh mục - Admin</title>
</svelte:head>

<div class="p-6">
	<div class="mb-6 flex items-center justify-between">
		<div>
			<h1 class="text-2xl font-bold text-gray-900">Quản lý Danh mục</h1>
			<p class="text-gray-600">Phân loại món ăn</p>
		</div>
		<Button variant="primary" onclick={() => vm.openCreateModal()}>+ Thêm danh mục</Button>
	</div>

	{#if vm.loading}
		<div class="py-12 text-center">
			<div
				class="inline-block h-12 w-12 animate-spin rounded-full border-b-2 border-indigo-600"
			></div>
		</div>
	{:else}
		<Card>
			<div class="overflow-x-auto">
				<table class="w-full">
					<thead class="border-b bg-gray-50">
						<tr>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Ảnh</th>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
								>Tên danh mục</th
							>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Mô tả</th>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
								>Trạng thái</th
							>
							<th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase"
								>Thao tác</th
							>
						</tr>
					</thead>
					<tbody class="divide-y">
						{#each vm.categories as item (item.id)}
							<tr class="hover:bg-gray-50">
								<td class="px-6 py-4">
									{#if item.imageUrl}
										<img
											src={item.imageUrl}
											alt={item.name}
											class="h-12 w-12 rounded-lg object-cover"
										/>
									{:else}
										<div
											class="flex h-12 w-12 items-center justify-center rounded-lg bg-gray-200 text-gray-400"
										>
											📂
										</div>
									{/if}
								</td>
								<td class="px-6 py-4 text-sm font-medium text-gray-900">{item.name}</td>
								<td class="px-6 py-4 text-sm text-gray-600">{item.description || '-'}</td>
								<td class="px-6 py-4">
									<span
										class="rounded-full px-2 py-1 text-xs {item.isActive
											? 'bg-green-100 text-green-800'
											: 'bg-gray-100 text-gray-800'}"
									>
										{item.isActive ? 'Hoạt động' : 'Tắt'}
									</span>
								</td>
								<td class="space-x-2 px-6 py-4 text-right">
									<button
										onclick={() => vm.openEditModal(item)}
										class="text-indigo-600 hover:text-indigo-900"
									>
										Sửa
									</button>
									<button
										onclick={() => vm.handleDelete(item.id)}
										class="text-red-600 hover:text-red-900"
									>
										Xóa
									</button>
								</td>
							</tr>
						{/each}
					</tbody>
				</table>
			</div>
		</Card>
	{/if}
</div>

<!-- Modal -->
<Modal
	open={vm.showModal}
	title={vm.editingItem ? 'Sửa danh mục' : 'Thêm danh mục'}
	onClose={() => (vm.showModal = false)}
>
	<form
		onsubmit={(e) => {
			e.preventDefault();
			vm.handleSubmit();
		}}
		class="space-y-4"
	>
		<div>
			<label for="catName" class="mb-2 block text-sm font-medium text-gray-700">Tên danh mục</label>
			<input
				id="catName"
				type="text"
				bind:value={vm.formData.name}
				required
				class="w-full rounded-lg border px-4 py-2 focus:ring-2 focus:ring-indigo-500"
				placeholder="Ví dụ: Món chính, Đồ uống..."
			/>
		</div>
		<div>
			<label for="catDesc" class="mb-2 block text-sm font-medium text-gray-700">Mô tả</label>
			<textarea
				id="catDesc"
				bind:value={vm.formData.description}
				rows="3"
				class="w-full rounded-lg border px-4 py-2 focus:ring-2 focus:ring-indigo-500"
				placeholder="Mô tả ngắn về danh mục này"
			></textarea>
		</div>
		<!-- Image Upload -->
		<div>
			<p class="mb-2 block text-sm font-medium text-gray-700">Ảnh danh mục</p>
			<div class="flex items-center gap-4">
				{#if vm.imagePreview || vm.formData.imageUrl}
					<img
						src={vm.imagePreview || vm.formData.imageUrl}
						alt="Preview"
						class="h-16 w-16 rounded-lg border object-cover"
					/>
				{:else}
					<div
						class="flex h-16 w-16 items-center justify-center rounded-lg border bg-gray-100 text-gray-400"
					>
						📂
					</div>
				{/if}
				<div class="flex flex-col gap-2">
					<button
						type="button"
						onclick={() => imageInput?.click()}
						class="rounded-lg bg-amber-100 px-3 py-1.5 text-sm text-amber-700 hover:bg-amber-200"
					>
						📷 Chọn ảnh
					</button>
					{#if vm.imagePreview || vm.formData.imageUrl}
						<button
							type="button"
							onclick={() => vm.clearImage()}
							class="px-3 py-1.5 text-sm text-red-600 hover:underline"
						>
							Xóa ảnh
						</button>
					{/if}
				</div>
			</div>
		</div>
		<div class="flex items-center">
			<input type="checkbox" bind:checked={vm.formData.isActive} id="isActive" class="mr-2" />
			<label for="isActive" class="text-sm text-gray-700">Hoạt động</label>
		</div>
		<div class="flex gap-3">
			<Button variant="primary" type="submit" fullWidth={true}>
				{vm.editingItem ? 'Cập nhật' : 'Tạo mới'}
			</Button>
			<Button variant="secondary" onclick={() => (vm.showModal = false)} fullWidth={true}
				>Hủy</Button
			>
		</div>
	</form>
</Modal>

<Toast bind:show={vm.showToast} message={vm.toastMessage} type={vm.toastType} />

<!-- Hidden File Input -->
<input
	type="file"
	accept="image/*"
	bind:this={imageInput}
	onchange={handleImageSelect}
	class="hidden"
/>

{#if vm.cropImageSrc}
	<ImageCropper
		imageSrc={vm.cropImageSrc}
		aspectRatio={1}
		onCrop={(blob) => vm.handleCrop(blob)}
		onCancel={() => vm.handleCancelCrop()}
	/>
{/if}
