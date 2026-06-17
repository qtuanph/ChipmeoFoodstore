<script lang="ts">
	import { onMount } from 'svelte';
	import { CustomersState } from './customers.svelte.js';
	import Icon from '$lib/components/ui/Icon.svelte';

	const vm = new CustomersState();

	onMount(() => {
		vm.init();
	});

	function formatDate(dateString?: string) {
		if (!dateString) return '-';
		return new Date(dateString).toLocaleDateString('vi-VN');
	}
</script>

<div class="p-6">
	<!-- Header -->
	<div class="mb-6 flex flex-col items-start justify-between gap-4 sm:flex-row sm:items-center">
		<div>
			<h1 class="text-2xl font-bold text-gray-900">Quản lý Khách hàng</h1>
			<p class="text-gray-600">Danh sách khách hàng đã đăng ký</p>
		</div>
		<div
			class="flex items-center gap-2 rounded-lg border border-gray-200 bg-white px-3 py-1.5 shadow-sm"
		>
			<span class="text-sm font-medium text-gray-900">{vm.customers.length}</span>
			<span class="text-sm text-gray-500">khách hàng</span>
		</div>
	</div>

	<!-- Search -->
	<div class="mb-6 rounded-xl border border-gray-200 bg-white p-4 shadow-sm">
		<div class="relative max-w-md">
			<Icon
				name="tabler:search"
				class="absolute top-1/2 left-3 h-5 w-5 -translate-y-1/2 text-gray-400"
			/>
			<input
				type="text"
				bind:value={vm.searchTerm}
				placeholder="Tìm theo tên, email, số điện thoại..."
				class="w-full rounded-lg border border-gray-300 py-2 pr-4 pl-10 transition-colors focus:border-amber-500 focus:ring-2 focus:ring-amber-500"
			/>
		</div>
	</div>

	<!-- Customer Table -->
	<div class="overflow-hidden rounded-xl border border-gray-200 bg-white shadow-sm">
		{#if vm.loading}
			<div class="p-12 text-center">
				<div
					class="inline-block h-8 w-8 animate-spin rounded-full border-b-2 border-amber-600"
				></div>
			</div>
		{:else if vm.filteredCustomers.length === 0}
			<div class="p-12 text-center text-gray-500">
				<div class="mb-4 text-4xl">👥</div>
				<p class="text-lg">Không tìm thấy khách hàng nào</p>
			</div>
		{:else}
			<div class="overflow-x-auto">
				<table class="min-w-full divide-y divide-gray-200">
					<thead class="bg-gray-50">
						<tr>
							<th
								class="px-6 py-3 text-left text-xs font-semibold tracking-wider text-gray-500 uppercase"
								>Khách hàng</th
							>
							<th
								class="px-6 py-3 text-left text-xs font-semibold tracking-wider text-gray-500 uppercase"
								>Liên hệ</th
							>
							<th
								class="px-6 py-3 text-left text-xs font-semibold tracking-wider text-gray-500 uppercase"
								>Điểm tích lũy</th
							>
							<th
								class="px-6 py-3 text-left text-xs font-semibold tracking-wider text-gray-500 uppercase"
								>Trạng thái</th
							>
							<th
								class="px-6 py-3 text-left text-xs font-semibold tracking-wider text-gray-500 uppercase"
								>Ngày đăng ký</th
							>
						</tr>
					</thead>
					<tbody class="divide-y divide-gray-200 bg-white">
						{#each vm.filteredCustomers as customer (customer.id)}
							<tr class="transition-colors hover:bg-gray-50">
								<td class="px-6 py-4 whitespace-nowrap">
									<div class="flex items-center gap-3">
										{#if customer.avatarUrl}
											<img
												src={customer.avatarUrl}
												alt=""
												class="h-10 w-10 rounded-full object-cover ring-2 ring-gray-100"
											/>
										{:else}
											<div
												class="flex h-10 w-10 items-center justify-center rounded-full border border-amber-200 bg-gradient-to-br from-amber-100 to-orange-100 font-bold text-amber-600"
											>
												{customer.fullName.charAt(0).toUpperCase()}
											</div>
										{/if}
										<div>
											<div class="font-medium text-gray-900">{customer.fullName}</div>
											<div class="font-mono text-xs text-gray-500">#{customer.id}</div>
										</div>
									</div>
								</td>
								<td class="px-6 py-4 whitespace-nowrap">
									<div class="text-sm text-gray-900">{customer.email}</div>
									<div class="text-sm text-gray-500">{customer.phone || 'Chưa cập nhật'}</div>
								</td>
								<td class="px-6 py-4 whitespace-nowrap">
									<span
										class="inline-flex items-center rounded-full border border-amber-100 bg-amber-50 px-2.5 py-0.5 text-xs font-medium text-amber-700"
									>
										{customer.points.toLocaleString()} điểm
									</span>
								</td>
								<td class="px-6 py-4 whitespace-nowrap">
									{#if customer.isActive}
										<span
											class="inline-flex items-center rounded-full border border-green-100 bg-green-50 px-2.5 py-0.5 text-xs font-medium text-green-700"
										>
											<span class="mr-1.5 h-1.5 w-1.5 rounded-full bg-green-500"></span>
											Hoạt động
										</span>
									{:else}
										<span
											class="inline-flex items-center rounded-full border border-red-100 bg-red-50 px-2.5 py-0.5 text-xs font-medium text-red-700"
										>
											<span class="mr-1.5 h-1.5 w-1.5 rounded-full bg-red-500"></span>
											Đã khóa
										</span>
									{/if}
								</td>
								<td class="px-6 py-4 text-sm whitespace-nowrap text-gray-500">
									{formatDate(customer.createdAt)}
								</td>
							</tr>
						{/each}
					</tbody>
				</table>
			</div>
		{/if}
	</div>
</div>
