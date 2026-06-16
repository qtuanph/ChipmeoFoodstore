<script lang="ts">
	import { onMount } from 'svelte';
	import { categories } from '$lib/utils/index.js';
	import { formatCurrency } from '$lib/utils/index.js';
	import Button from '$lib/components/Button.svelte';
	import Card from '$lib/components/Card.svelte';
	import Modal from '$lib/components/Modal.svelte';
	import Toast from '$lib/components/Toast.svelte';
	import ImageCropper from '$lib/components/ImageCropper.svelte';
	import { MenuState } from './menu.svelte.js';

	const vm = new MenuState();
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
	<title>Quản lý Menu - Admin</title>
</svelte:head>

<div class="p-6">
	<div class="items-cent mb-6 flex justify-between">
		<div>
			<h1 class="text-2xl font-bold text-gray-900">Quản lý Menu</h1>
			<p class="text-gray-600">Danh sách món ăn</p>
		</div>
		<Button variant="primary" onclick={() => vm.openCreateModal()}>+ Thêm món mới</Button>
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
								>Tên món</th
							>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
								>Danh mục</th
							>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Giá</th>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
								>Trạng thái</th
							>
							<th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase"
								>Thao tác</th
							>
						</tr>
					</thead>
					<tbody class="divide-y">
						{#each vm.menuItems as item (item.id)}
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
											🍽️
										</div>
									{/if}
								</td>
								<td class="px-6 py-4 text-sm font-medium text-gray-900">{item.name}</td>
								<td class="px-6 py-4 text-sm text-gray-600"
									>{vm.getCategoryName(item.categoryId || 0)}</td
								>
								<td class="px-6 py-4 text-sm text-gray-900">{formatCurrency(item.price)}</td>
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
	title={vm.editingItem ? 'Sửa món' : 'Thêm món mới'}
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
			<label for="itemName" class="mb-2 block text-sm font-medium text-gray-700">Tên món</label>
			<input
				id="itemName"
				type="text"
				bind:value={vm.formData.name}
				required
				class="w-full rounded-lg border px-4 py-2 focus:ring-2 focus:ring-indigo-500"
			/>
		</div>
		<div>
			<label for="itemCategory" class="mb-2 block text-sm font-medium text-gray-700">Danh mục</label
			>
			<select
				id="itemCategory"
				bind:value={vm.formData.categoryId}
				required
				class="w-full rounded-lg border px-4 py-2 focus:ring-2 focus:ring-indigo-500"
			>
				{#each $categories as cat (cat.id)}
					<option value={cat.id}>{cat.name}</option>
				{/each}
			</select>
		</div>
		<div>
			<label for="itemPrice" class="mb-2 block text-sm font-medium text-gray-700">Giá</label>
			<input
				id="itemPrice"
				type="number"
				bind:value={vm.formData.price}
				required
				min="0"
				class="w-full rounded-lg border px-4 py-2 focus:ring-2 focus:ring-indigo-500"
			/>
		</div>

		<!-- Image Selection -->
		<div>
			<p class="mb-2 block text-sm font-medium text-gray-700">Ảnh món ăn</p>
			<div class="flex items-center gap-4">
				{#if vm.imagePreview || vm.formData.imageUrl}
					<img
						src={vm.imagePreview || vm.formData.imageUrl}
						alt="Preview"
						class="h-20 w-20 rounded-lg border object-cover"
					/>
				{:else}
					<div
						class="flex h-20 w-20 items-center justify-center rounded-lg border bg-gray-100 text-gray-400"
					>
						🖼️
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

		<!-- Addons Selection -->
		<div>
			<p class="mb-2 block text-sm font-medium text-gray-700">Topping / Món thêm</p>
			<div class="max-h-40 space-y-2 overflow-y-auto rounded-lg border bg-gray-50 p-3">
				{#each vm.allAddons as addon (addon.id)}
					<div class="flex items-center justify-between">
						<div class="flex items-center">
							<input
								type="checkbox"
								id="addon-{addon.id}"
								checked={vm.formData.addonIds.includes(addon.id)}
								onchange={() => vm.toggleAddon(addon.id)}
								class="mr-2"
							/>
							<label for="addon-{addon.id}" class="text-sm text-gray-700">{addon.name}</label>
						</div>
						<span class="text-xs text-gray-500">{formatCurrency(addon.price)}</span>
					</div>
				{/each}
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
