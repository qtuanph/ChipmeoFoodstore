<script lang="ts">
	import { onMount } from 'svelte';
	import { formatCurrency } from '$lib/utils/index.js';
	import Button from '$lib/components/ui/Button.svelte';
	import Card from '$lib/components/ui/Card.svelte';
	import Modal from '$lib/components/ui/Modal.svelte';
	import Toast from '$lib/components/ui/Toast.svelte';
	import { DiscountsState } from './discounts.svelte.js';

	const vm = new DiscountsState();

	onMount(() => {
		vm.init();
	});
</script>

<svelte:head><title>Quản lý Giảm giá - Admin</title></svelte:head>

<div class="p-6">
	<div class="mb-6 flex items-center justify-between">
		<div>
			<h1 class="text-2xl font-bold">Quản lý Giảm giá</h1>
			<p class="text-gray-600">Mã giảm giá & khuyến mãi</p>
		</div>
		<Button variant="primary" onclick={() => vm.openCreateModal()}>+ Thêm mã giảm giá</Button>
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
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Mã</th>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Tên</th>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Loại</th>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
								>Giá trị</th
							>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
								>Thời hạn</th
							>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
								>Trạng thái</th
							>
							<th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase"
								>Thao tác</th
							>
						</tr>
					</thead>
					<tbody class="divide-y">
						{#each vm.discounts as item (item.id)}
							<tr class="hover:bg-gray-50">
								<td class="px-6 py-4 text-sm font-medium">{item.code}</td>
								<td class="px-6 py-4 text-sm">{item.name}</td>
								<td class="px-6 py-4 text-sm"
									>{item.type === 'percent' ? 'Phần trăm' : 'Số tiền'}</td
								>
								<td class="px-6 py-4 text-sm font-semibold"
									>{item.type === 'percent' ? `${item.value}%` : formatCurrency(item.value)}</td
								>
								<td class="px-6 py-4 text-sm text-gray-500">
									{#if item.startDate || item.endDate}
										<div class="text-xs">
											{item.startDate
												? new Date(item.startDate).toLocaleDateString('vi-VN')
												: '...'}
											-
											{item.endDate ? new Date(item.endDate).toLocaleDateString('vi-VN') : '...'}
										</div>
									{:else}
										Vô thời hạn
									{/if}
								</td>
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
										class="text-indigo-600 hover:text-indigo-900">Sửa</button
									>
									<button
										onclick={() => vm.handleDelete(item.id)}
										class="text-red-600 hover:text-red-900">Xóa</button
									>
								</td>
							</tr>
						{/each}
					</tbody>
				</table>
			</div>
		</Card>
	{/if}
</div>

<Modal
	open={vm.showModal}
	title={vm.editingItem ? 'Sửa mã giảm giá' : 'Thêm mã giảm giá'}
	onClose={() => (vm.showModal = false)}
>
	<form
		onsubmit={(e) => {
			e.preventDefault();
			vm.handleSubmit();
		}}
		class="space-y-4"
	>
		<div class="grid grid-cols-2 gap-4">
			<div>
				<label for="code" class="mb-2 block text-sm font-medium">Mã code</label>
				<input
					id="code"
					type="text"
					bind:value={vm.formData.code}
					required
					class="w-full rounded-lg border px-4 py-2"
				/>
			</div>
			<div>
				<label for="name" class="mb-2 block text-sm font-medium">Tên</label>
				<input
					id="name"
					type="text"
					bind:value={vm.formData.name}
					required
					class="w-full rounded-lg border px-4 py-2"
				/>
			</div>
		</div>

		<div class="grid grid-cols-2 gap-4">
			<div>
				<label for="type" class="mb-2 block text-sm font-medium">Loại giảm giá</label>
				<select id="type" bind:value={vm.formData.type} class="w-full rounded-lg border px-4 py-2">
					<option value="percent">Phần trăm</option>
					<option value="amount">Số tiền</option>
				</select>
			</div>
			<div>
				<label for="value" class="mb-2 block text-sm font-medium">Giá trị</label>
				<input
					id="value"
					type="number"
					bind:value={vm.formData.value}
					required
					min="0"
					class="w-full rounded-lg border px-4 py-2"
				/>
			</div>
		</div>

		<div class="grid grid-cols-2 gap-4">
			<div>
				<label for="minOrderAmount" class="mb-2 block text-sm font-medium">Đơn tối thiểu</label>
				<input
					id="minOrderAmount"
					type="number"
					bind:value={vm.formData.minOrderAmount}
					min="0"
					class="w-full rounded-lg border px-4 py-2"
				/>
			</div>
			<div>
				<label for="maxDiscountAmount" class="mb-2 block text-sm font-medium">Giảm tối đa</label>
				<input
					id="maxDiscountAmount"
					type="number"
					bind:value={vm.formData.maxDiscountAmount}
					min="0"
					class="w-full rounded-lg border px-4 py-2"
				/>
			</div>
		</div>

		<div class="grid grid-cols-2 gap-4">
			<div>
				<label for="startDate" class="mb-2 block text-sm font-medium">Ngày bắt đầu</label>
				<input
					id="startDate"
					type="datetime-local"
					bind:value={vm.formData.startDate}
					class="w-full rounded-lg border px-4 py-2"
				/>
			</div>
			<div>
				<label for="endDate" class="mb-2 block text-sm font-medium">Ngày kết thúc</label>
				<input
					id="endDate"
					type="datetime-local"
					bind:value={vm.formData.endDate}
					class="w-full rounded-lg border px-4 py-2"
				/>
			</div>
		</div>

		<div>
			<label for="usageLimit" class="mb-2 block text-sm font-medium"
				>Giới hạn sử dụng (0 = KGH)</label
			>
			<input
				id="usageLimit"
				type="number"
				bind:value={vm.formData.usageLimit}
				min="0"
				class="w-full rounded-lg border px-4 py-2"
			/>
		</div>

		<div class="flex items-center">
			<input type="checkbox" bind:checked={vm.formData.isActive} id="isActive" class="mr-2" />
			<label for="isActive" class="text-sm">Hoạt động</label>
		</div>
		<div class="flex gap-3">
			<Button variant="primary" type="submit" fullWidth={true}
				>{vm.editingItem ? 'Cập nhật' : 'Tạo mới'}</Button
			>
			<Button variant="secondary" onclick={() => (vm.showModal = false)} fullWidth={true}
				>Hủy</Button
			>
		</div>
	</form>
</Modal>

<Toast bind:show={vm.showToast} message={vm.toastMessage} type={vm.toastType} />
