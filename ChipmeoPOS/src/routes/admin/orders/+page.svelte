<script lang="ts">
	import { onMount, onDestroy } from 'svelte';
	import { formatCurrency } from '$lib/utils/index.js';
	import Button from '$lib/components/Button.svelte';
	import Card from '$lib/components/Card.svelte';
	import Toast from '$lib/components/Toast.svelte';
	import Modal from '$lib/components/Modal.svelte';
	import OrderHistory from '$lib/components/OrderHistory.svelte';
	import { OrdersState } from './orders.svelte.js';

	const vm = new OrdersState();

	onMount(() => {
		vm.init();
	});

	onDestroy(() => {
		vm.onDestroy();
	});
</script>

<svelte:head><title>Quản lý Đơn hàng - Admin</title></svelte:head>

<div class="p-6">
	<div class="mb-6 flex flex-col justify-between gap-4 md:flex-row md:items-center">
		<div>
			<h1 class="text-2xl font-bold">Quản lý Đơn hàng</h1>
			<p class="text-gray-600">Danh sách đơn hàng ({vm.totalCount} đơn)</p>
		</div>

		<!-- Filters -->
		<div class="flex flex-wrap items-end gap-3">
			<div>
				<label for="fromDate" class="mb-1 block text-xs font-medium text-gray-700">Từ ngày</label>
				<input
					id="fromDate"
					type="date"
					bind:value={vm.fromDate}
					class="rounded-lg border px-3 py-2 text-sm"
				/>
			</div>
			<div>
				<label for="toDate" class="mb-1 block text-xs font-medium text-gray-700">Đến ngày</label>
				<input
					id="toDate"
					type="date"
					bind:value={vm.toDate}
					class="rounded-lg border px-3 py-2 text-sm"
				/>
			</div>
			<Button variant="primary" onclick={() => vm.handleFilter()}>Lọc</Button>
		</div>
	</div>

	{#if vm.loading}
		<div class="py-12 text-center">
			<div
				class="inline-block h-12 w-12 animate-spin rounded-full border-b-2 border-indigo-600"
			></div>
		</div>
	{:else}
		<Card>
			<!-- Page Size Selector -->
			<div class="flex items-center justify-between border-b p-4">
				<div class="flex items-center gap-2">
					<span class="text-sm text-gray-600">Hiển thị:</span>
					<select
						bind:value={vm.pageSize}
						onchange={() => {
							vm.page = 1;
							vm.loadData();
						}}
						class="rounded border px-2 py-1 text-sm focus:border-indigo-500 focus:ring-indigo-500"
					>
						<option value={10}>10</option>
						<option value={20}>20</option>
						<option value={30}>30</option>
						<option value={50}>50</option>
					</select>
				</div>

				<!-- Pagination Controls (Top) -->
				<div class="flex gap-2">
					<button
						onclick={() => vm.handlePageChange(vm.page - 1)}
						disabled={vm.page === 1}
						class="rounded border px-3 py-1 hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-50"
					>
						&lt;
					</button>
					<span class="px-3 py-1 text-sm font-medium">Trang {vm.page} / {vm.totalPages}</span>
					<button
						onclick={() => vm.handlePageChange(vm.page + 1)}
						disabled={vm.page === vm.totalPages}
						class="rounded border px-3 py-1 hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-50"
					>
						&gt;
					</button>
				</div>
			</div>

			<div class="overflow-x-auto">
				<table class="w-full">
					<thead class="border-b bg-gray-50">
						<tr>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Mã đơn</th
							>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
								>Nguồn đơn</th
							>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
								>Tổng tiền</th
							>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
								>Trạng thái</th
							>
							<th class="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase"
								>Thời gian</th
							>
							<th class="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase"
								>Thao tác</th
							>
						</tr>
					</thead>
					<tbody class="divide-y">
						{#each vm.orders as order (order.id)}
							<tr class="hover:bg-gray-50">
								<td class="px-6 py-4 text-sm font-medium">{order.orderCode}</td>
								<td class="px-6 py-4 text-sm">{order.sourceName || order.source?.name || 'N/A'}</td>
								<td class="px-6 py-4 text-sm font-semibold">{formatCurrency(order.totalAmount)}</td>
								<td class="px-6 py-4">
									<span class="rounded-full px-2 py-1 text-xs {vm.getStatusBadge(order.status)}"
										>{order.status}</span
									>
								</td>
								<td class="px-6 py-4 text-sm text-gray-600"
									>{new Date(order.createdAt).toLocaleString('vi-VN')}</td
								>
								<td class="flex justify-end gap-2 px-6 py-4 text-right">
									<button
										onclick={() => vm.openDetails(order)}
										class="text-sm font-medium text-blue-600 hover:text-blue-900"
									>
										Chi tiết
									</button>
									<button
										onclick={() => {
											vm.selectedOrder = order;
											vm.showHistoryModal = true;
										}}
										class="text-sm font-medium text-indigo-600 hover:text-indigo-900"
									>
										Lịch sử
									</button>
									{#if order.status === 'paid'}
										<button
											onclick={() => vm.handleSetUnpaid(order.id)}
											class="flex items-center gap-1 text-sm font-medium text-orange-600 hover:text-orange-900"
											title="Đặt về trạng thái chưa thanh toán"
										>
											<svg class="h-4 w-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
												<path
													stroke-linecap="round"
													stroke-linejoin="round"
													stroke-width="2"
													d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
												/>
											</svg>
											Chưa thanh toán
										</button>
									{/if}
								</td>
							</tr>
						{/each}
					</tbody>
				</table>
			</div>

			<!-- Pagination Controls (Bottom) -->
			<div class="flex justify-end border-t p-4">
				<div class="flex gap-2">
					<button
						onclick={() => vm.handlePageChange(vm.page - 1)}
						disabled={vm.page === 1}
						class="rounded border px-3 py-1 hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-50"
					>
						Trước
					</button>
					{#each Array(vm.totalPages) as _, i (i)}
						{#if i + 1 >= vm.page - 2 && i + 1 <= vm.page + 2}
							<button
								onclick={() => vm.handlePageChange(i + 1)}
								class="rounded border px-3 py-1 {vm.page === i + 1
									? 'bg-indigo-600 text-white'
									: 'hover:bg-gray-50'}"
							>
								{i + 1}
							</button>
						{/if}
					{/each}
					<button
						onclick={() => vm.handlePageChange(vm.page + 1)}
						disabled={vm.page === vm.totalPages}
						class="rounded border px-3 py-1 hover:bg-gray-50 disabled:cursor-not-allowed disabled:opacity-50"
					>
						Sau
					</button>
				</div>
			</div>
		</Card>
	{/if}
</div>

<Toast bind:show={vm.showToast} message={vm.toastMessage} type={vm.toastType} />

<!-- Confirm Unpaid Modal -->
<Modal
	bind:open={vm.showConfirmModal}
	title="Xác nhận thay đổi trạng thái"
	onClose={() => (vm.showConfirmModal = false)}
>
	<div class="p-4">
		<p class="mb-4 text-gray-700">
			Bạn có chắc chắn muốn chuyển đơn hàng <span class="font-bold"
				>#{vm.selectedOrderIdToUnpaid}</span
			>
			về trạng thái <span class="font-bold text-yellow-600">Chưa thanh toán</span>?
		</p>
		<p class="mb-6 text-sm text-gray-500">
			Hành động này sẽ cho phép nhân viên thu ngân thực hiện lại thanh toán cho đơn hàng này.
		</p>

		<div class="flex justify-end gap-3">
			<Button variant="secondary" onclick={() => (vm.showConfirmModal = false)}>Hủy</Button>
			<Button variant="primary" onclick={() => vm.confirmSetUnpaid()}>Xác nhận</Button>
		</div>
	</div>
</Modal>

<Modal
	bind:open={vm.showHistoryModal}
	title="Lịch sử đơn hàng #{vm.selectedOrder?.orderCode}"
	onClose={() => (vm.showHistoryModal = false)}
>
	{#if vm.selectedOrder && vm.selectedOrder.history}
		<OrderHistory history={vm.selectedOrder.history} />
	{:else}
		<p class="text-center text-gray-500">Không có dữ liệu lịch sử</p>
	{/if}
</Modal>

<!-- Order Details Modal -->
<Modal
	bind:open={vm.showDetailsModal}
	title="Chi tiết đơn hàng #{vm.selectedOrder?.orderCode}"
	onClose={() => (vm.showDetailsModal = false)}
>
	{#if vm.selectedOrder}
		<div class="space-y-4">
			<div class="flex items-start justify-between rounded-lg bg-gray-50 p-3">
				<div>
					<p class="text-sm text-gray-500">Khách hàng</p>
					<p class="font-medium">{vm.selectedOrder.customerName || 'Khách lẻ'}</p>
					{#if vm.selectedOrder.customerPhone}
						<p class="text-sm text-gray-500">{vm.selectedOrder.customerPhone}</p>
					{/if}
				</div>
				<div class="text-right">
					<p class="text-sm text-gray-500">Nguồn đơn</p>
					<p class="font-medium">
						{vm.selectedOrder.sourceName || vm.selectedOrder.source?.name || 'Mang về'}
					</p>
				</div>
			</div>

			<div class="overflow-hidden rounded-lg border">
				<table class="w-full text-sm">
					<thead class="border-b bg-gray-50">
						<tr>
							<th class="px-4 py-2 text-left">Tên món</th>
							<th class="px-4 py-2 text-center">SL</th>
							<th class="px-4 py-2 text-right">Đơn giá</th>
							<th class="px-4 py-2 text-right">Thành tiền</th>
						</tr>
					</thead>
					<tbody class="divide-y">
						{#each vm.selectedOrder.items || [] as item (item.id || item.menuItemName)}
							<tr>
								<td class="px-4 py-3">
									<div class="font-medium">
										{item.menuItemName}
										{#if item.comboId}
											<span
												class="ml-2 rounded-full bg-orange-100 px-2 py-0.5 text-xs text-orange-600"
												>Combo</span
											>
										{/if}
									</div>
									{#if item.addons && item.addons.length > 0}
										<div class="mt-1 border-l-2 border-gray-200 pl-2 text-xs text-gray-500">
											{#each item.addons as addon, i (i)}
												<div>+ {addon.addonName} (x{addon.quantity})</div>
											{/each}
										</div>
									{/if}
									{#if item.note}
										<div class="mt-1 text-xs text-gray-500 italic">"{item.note}"</div>
									{/if}
								</td>
								<td class="px-4 py-3 text-center">{item.quantity}</td>
								<td class="px-4 py-3 text-right">{formatCurrency(item.unitPrice)}</td>
								<td class="px-4 py-3 text-right font-medium">{formatCurrency(item.totalPrice)}</td>
							</tr>
						{/each}
					</tbody>
				</table>
			</div>

			<div class="space-y-2 border-t pt-2">
				<div class="flex justify-between text-sm">
					<span class="text-gray-600">Tạm tính:</span>
					<span>{formatCurrency(vm.selectedOrder.subtotalAmount)}</span>
				</div>
				{#if vm.selectedOrder.discountAmount > 0}
					<div class="flex justify-between text-sm text-green-600">
						<span>Giảm giá ({vm.selectedOrder.discountCode || ''}):</span>
						<span>-{formatCurrency(vm.selectedOrder.discountAmount)}</span>
					</div>
				{/if}
				<div class="flex justify-between border-t pt-2 text-lg font-bold text-indigo-600">
					<span>Tổng cộng:</span>
					<span>{formatCurrency(vm.selectedOrder.totalAmount)}</span>
				</div>
			</div>

			{#if vm.selectedOrder.note}
				<div class="rounded-lg border border-yellow-100 bg-yellow-50 p-3 text-sm text-yellow-800">
					<span class="font-semibold">Ghi chú đơn hàng:</span>
					{vm.selectedOrder.note}
				</div>
			{/if}
		</div>
	{/if}
</Modal>
