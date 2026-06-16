<script lang="ts">
	import { onMount, onDestroy } from 'svelte';
	import { formatCurrency } from '$lib/utils/index.ts';
	import { signalRService } from '$lib/services/signalr.ts';
	import { DashboardState } from './dashboard.svelte.js';

	const vm = new DashboardState();

	// UI Local State - only canvas refs and chart instances
	let revenueChartDiv: HTMLDivElement;
	let popularItemsChartDiv: HTMLDivElement;
	let popularCombosChartDiv: HTMLDivElement;
	let paymentMethodsChartDiv: HTMLDivElement;
	let serviceTypeChartDiv: HTMLDivElement;

	onMount(async () => {
		await vm.loadData();

		// Initialize charts after data is loaded and DOM is updated
		requestAnimationFrame(() => {
			vm.initCharts({
				revenue: revenueChartDiv,
				popularItems: popularItemsChartDiv,
				popularCombos: popularCombosChartDiv,
				paymentMethods: paymentMethodsChartDiv,
				serviceType: serviceTypeChartDiv
			});
		});

		// Start SignalR connection
		const token = localStorage.getItem('token');
		if (token) {
			await signalRService.startConnection(token);
			signalRService.on('ReceiveOrderUpdate', handleOrderUpdate);
		}
	});

	onDestroy(() => {
		signalRService.off('ReceiveOrderUpdate', handleOrderUpdate);
	});

	function handleOrderUpdate(data: any) {
		vm.handleOrderUpdate(data);
		// Re-initialize charts after data reload
		setTimeout(() => {
			vm.initCharts({
				revenue: revenueChartDiv,
				popularItems: popularItemsChartDiv,
				popularCombos: popularCombosChartDiv,
				paymentMethods: paymentMethodsChartDiv,
				serviceType: serviceTypeChartDiv
			});
		}, 100);
	}

	async function onRefresh() {
		await vm.loadData();
		// Re-initialize charts
		requestAnimationFrame(() => {
			vm.initCharts({
				revenue: revenueChartDiv,
				popularItems: popularItemsChartDiv,
				popularCombos: popularCombosChartDiv,
				paymentMethods: paymentMethodsChartDiv,
				serviceType: serviceTypeChartDiv
			});
		});
	}
</script>

<svelte:head>
	<title>Tổng quan - Admin Chipmeo</title>
</svelte:head>

<div class="space-y-6 p-6">
	{#if vm.loading}
		<div class="flex h-64 items-center justify-center">
			<div class="h-12 w-12 animate-spin rounded-full border-b-2 border-indigo-600"></div>
		</div>
	{:else if vm.error}
		<div
			class="relative rounded border border-red-200 bg-red-50 px-4 py-3 text-red-700"
			role="alert"
		>
			<strong class="font-bold">Lỗi!</strong>
			<span class="block sm:inline">{vm.error}</span>
		</div>
	{:else if vm.stats}
		<!-- Overview Cards -->
		<div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-4">
			<!-- Today Revenue -->
			<div
				class="group relative overflow-hidden rounded-2xl border border-gray-100 bg-white p-6 shadow-sm transition-all hover:shadow-md"
			>
				<div class="flex items-center justify-between">
					<div class="rounded-xl bg-emerald-50 p-3 text-emerald-600">
						<svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
							<path
								stroke-linecap="round"
								stroke-linejoin="round"
								stroke-width="2"
								d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
							/>
						</svg>
					</div>
					<div class="text-right">
						<p class="text-xs font-bold tracking-wider text-gray-400 uppercase">Hôm nay</p>
						<p class="mt-1 text-2xl font-black text-gray-900">
							{formatCurrency(vm.stats.today.revenue)}
						</p>
					</div>
				</div>
				<div class="mt-4 flex items-center justify-between text-sm">
					<span class="font-medium text-gray-500">Số đơn hàng</span>
					<span class="font-bold text-emerald-600">{vm.stats.today.orders} đơn</span>
				</div>
			</div>

			<!-- Month Revenue -->
			<div
				class="group relative overflow-hidden rounded-2xl border border-gray-100 bg-white p-6 shadow-sm transition-all hover:shadow-md"
			>
				<div class="flex items-center justify-between">
					<div class="rounded-xl bg-blue-50 p-3 text-blue-600">
						<svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
							<path
								stroke-linecap="round"
								stroke-linejoin="round"
								stroke-width="2"
								d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
							/>
						</svg>
					</div>
					<div class="text-right">
						<p class="text-xs font-bold tracking-wider text-gray-400 uppercase">Tháng này</p>
						<p class="mt-1 text-2xl font-black text-gray-900">
							{formatCurrency(vm.stats.month.revenue)}
						</p>
					</div>
				</div>
				<div class="mt-4 flex items-center justify-between text-sm">
					<span class="font-medium text-gray-500">Tổng đơn hàng</span>
					<span class="font-bold text-blue-600">{vm.stats.month.orders} đơn</span>
				</div>
			</div>

			<!-- Total Customers -->
			<div
				class="group relative overflow-hidden rounded-2xl border border-gray-100 bg-white p-6 shadow-sm transition-all hover:shadow-md"
			>
				<div class="flex items-center justify-between">
					<div class="rounded-xl bg-purple-50 p-3 text-purple-600">
						<svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
							<path
								stroke-linecap="round"
								stroke-linejoin="round"
								stroke-width="2"
								d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"
							/>
						</svg>
					</div>
					<div class="text-right">
						<p class="text-xs font-bold tracking-wider text-gray-400 uppercase">Khách hàng</p>
						<p class="mt-1 text-2xl font-black text-gray-900">{vm.stats.totalCustomers}</p>
					</div>
				</div>
				<div class="mt-4 flex items-center justify-between text-sm">
					<span class="font-medium text-gray-500">TB mỗi đơn</span>
					<span class="font-bold text-purple-600">{formatCurrency(vm.stats.averageOrderValue)}</span
					>
				</div>
			</div>

			<!-- Peak Hour -->
			<div
				class="group relative overflow-hidden rounded-2xl border border-gray-100 bg-white p-6 shadow-sm transition-all hover:shadow-md"
			>
				<div class="flex items-center justify-between">
					<div class="rounded-xl bg-orange-50 p-3 text-orange-600">
						<svg class="h-6 w-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
							<path
								stroke-linecap="round"
								stroke-linejoin="round"
								stroke-width="2"
								d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
							/>
						</svg>
					</div>
					<div class="text-right">
						<p class="text-xs font-bold tracking-wider text-gray-400 uppercase">Giờ cao điểm</p>
						<p class="mt-1 text-2xl font-black text-gray-900">{vm.stats.peakHour}</p>
					</div>
				</div>
				<div class="mt-4 flex items-center justify-between text-sm">
					<span class="font-medium text-gray-500">Dự kiến</span>
					<span class="font-bold text-orange-600">Theo lịch sử</span>
				</div>
			</div>
		</div>

		<!-- Charts Section -->
		<div class="grid grid-cols-1 gap-6 lg:grid-cols-3">
			<!-- Revenue Chart -->
			<div class="rounded-2xl border border-gray-100 bg-white p-6 shadow-sm lg:col-span-2">
				<div class="mb-6 flex items-center justify-between">
					<h3 class="text-lg text-[15px] font-bold text-gray-900">
						Biểu đồ doanh thu (7 ngày qua)
					</h3>
					<div
						class="rounded-lg bg-indigo-50 px-2 py-1 text-[10px] font-bold tracking-tight text-indigo-600 uppercase"
					>
						AI Predicted
					</div>
				</div>
				<div bind:this={revenueChartDiv} class="h-[400px] w-full"></div>
			</div>

			<!-- Right Column Charts -->
			<div class="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-1">
				<!-- Payment Methods -->
				<div class="rounded-2xl border border-gray-100 bg-white p-6 shadow-sm">
					<h3 class="mb-4 text-xs font-bold tracking-wider text-gray-400 uppercase">Thanh toán</h3>
					<div bind:this={paymentMethodsChartDiv} class="h-48 w-full"></div>
				</div>

				<!-- Order Source (Today) -->
				<div class="rounded-2xl border border-gray-100 bg-white p-6 shadow-sm">
					<h3 class="mb-4 text-xs font-bold tracking-wider text-gray-400 uppercase">Nguồn đơn</h3>
					<div bind:this={serviceTypeChartDiv} class="h-48 w-full"></div>
				</div>
			</div>
		</div>

		<!-- Popular Items & Combos -->
		<div class="grid grid-cols-1 gap-6 lg:grid-cols-2">
			<!-- Popular Items -->
			<div class="rounded-2xl border border-gray-100 bg-white p-6 shadow-sm">
				<h3 class="mb-6 text-lg text-[15px] font-bold text-gray-900">Món bán chạy (Hôm nay)</h3>
				<div bind:this={popularItemsChartDiv} class="mb-6 h-64"></div>
				<div class="overflow-x-auto">
					<table class="min-w-full divide-y divide-gray-200">
						<thead class="bg-gray-50">
							<tr>
								<th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase"
									>Tên món</th
								>
								<th class="px-4 py-2 text-right text-xs font-medium text-gray-500 uppercase">SL</th>
								<th class="px-4 py-2 text-right text-xs font-medium text-gray-500 uppercase"
									>Doanh thu</th
								>
							</tr>
						</thead>
						<tbody class="divide-y divide-gray-200">
							{#each vm.stats.popularItems as item (item.name)}
								<tr>
									<td class="px-4 py-2 text-sm text-gray-900">{item.name}</td>
									<td class="px-4 py-2 text-right text-sm text-gray-500">{item.quantity}</td>
									<td class="px-4 py-2 text-right text-sm text-gray-500"
										>{formatCurrency(item.revenue)}</td
									>
								</tr>
							{/each}
						</tbody>
					</table>
				</div>
			</div>

			<!-- Popular Combos -->
			<div class="rounded-2xl border border-gray-100 bg-white p-6 shadow-sm">
				<h3 class="mb-6 text-lg text-[15px] font-bold text-gray-900">Combo bán chạy (Hôm nay)</h3>
				<div bind:this={popularCombosChartDiv} class="mb-6 h-64"></div>
				<div class="overflow-x-auto">
					<table class="min-w-full divide-y divide-gray-200">
						<thead class="bg-gray-50">
							<tr>
								<th class="px-4 py-2 text-left text-xs font-medium text-gray-500 uppercase"
									>Tên combo</th
								>
								<th class="px-4 py-2 text-right text-xs font-medium text-gray-500 uppercase">SL</th>
								<th class="px-4 py-2 text-right text-xs font-medium text-gray-500 uppercase"
									>Doanh thu</th
								>
							</tr>
						</thead>
						<tbody class="divide-y divide-gray-200">
							{#if vm.stats.popularCombos && vm.stats.popularCombos.length > 0}
								{#each vm.stats.popularCombos as item (item.name)}
									<tr>
										<td class="px-4 py-2 text-sm text-gray-900">{item.name}</td>
										<td class="px-4 py-2 text-right text-sm text-gray-500">{item.quantity}</td>
										<td class="px-4 py-2 text-right text-sm text-gray-500"
											>{formatCurrency(item.revenue)}</td
										>
									</tr>
								{/each}
							{:else}
								<tr>
									<td colspan="3" class="px-4 py-4 text-center text-sm text-gray-500"
										>Chưa có dữ liệu combo</td
									>
								</tr>
							{/if}
						</tbody>
					</table>
				</div>
			</div>
		</div>
	{/if}
</div>
