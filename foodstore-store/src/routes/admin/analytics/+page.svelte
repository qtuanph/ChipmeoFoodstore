<script lang="ts">
	import { onMount, onDestroy } from 'svelte';
	import { formatCurrency } from '$lib/utils/index.ts';
	import { signalRService } from '$lib/services/signalr.ts';
	import { AnalyticsState } from './analytics.svelte.js';
	import Icon from '$lib/components/ui/Icon.svelte';

	const vm = new AnalyticsState();

	// UI Local State - only canvas refs
	let revenueChartDiv: HTMLDivElement = $state()!;
	let ordersChartDiv: HTMLDivElement = $state()!;
	let forecastChartDiv: HTMLDivElement = $state()!;
	let sourceChartDiv: HTMLDivElement = $state()!;

	// Reactive: update dates when dateRange changes

	// Re-initialize charts when data changes
	$effect(() => {
		if (!vm.loading && vm.stats) {
			setTimeout(() => {
				vm.initCharts({
					revenue: revenueChartDiv,
					orders: ordersChartDiv,
					forecast: forecastChartDiv,
					source: sourceChartDiv
				});
			}, 100);
		}
	});

	onMount(async () => {
		vm.updateDatesFromRange('7days');
		await vm.loadData();

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
	}

	async function onFilterApply() {
		await vm.loadData();
	}
</script>

<svelte:head>
	<title>Thống Kê & Dự Báo - Admin Foodstore</title>
</svelte:head>

<div class="space-y-6 p-6">
	<div class="flex items-center justify-between">
		<!-- Filters -->
		<div
			class="flex items-center gap-3 rounded-2xl border border-gray-100 bg-white p-1.5 shadow-sm"
		>
			<select
				bind:value={vm.dateRange}
				onchange={() => vm.dateRange !== 'custom' && vm.updateDatesFromRange(vm.dateRange)}
				class="cursor-pointer rounded-xl border-transparent bg-transparent px-4 py-2 text-sm font-bold text-gray-700 transition-colors hover:bg-gray-50 focus:ring-0"
			>
				<option value="7days">7 ngày qua</option>
				<option value="30days">30 ngày qua</option>
				<option value="thisMonth">Tháng này</option>
				<option value="last3Months">3 tháng qua</option>
				<option value="custom">Tùy chỉnh</option>
			</select>

			{#if vm.dateRange === 'custom'}
				<div class="flex items-center gap-2 border-l border-gray-100 px-2">
					<input
						type="date"
						bind:value={vm.fromDate}
						class="rounded-lg border-gray-100 py-1 text-xs font-medium"
					/>
					<span class="text-gray-300">/</span>
					<input
						type="date"
						bind:value={vm.toDate}
						class="rounded-lg border-gray-100 py-1 text-xs font-medium"
					/>
					<button
						onclick={onFilterApply}
						class="rounded-lg bg-indigo-600 px-3 py-1 text-xs font-bold text-white transition-colors hover:bg-indigo-700"
					>
						Lọc
					</button>
				</div>
			{/if}
		</div>
	</div>

	{#if vm.loading}
		<div class="flex h-64 items-center justify-center">
			<div class="h-12 w-12 animate-spin rounded-full border-b-2 border-indigo-600"></div>
		</div>
	{:else if vm.error}
		<div class="relative rounded border border-red-200 bg-red-50 px-4 py-3 text-red-700">
			{vm.error}
		</div>
	{:else if vm.stats}
		<!-- Key Metrics -->
		<div class="grid grid-cols-1 gap-6 md:grid-cols-3">
			<div
				class="group rounded-2xl border border-gray-100 bg-white p-6 shadow-sm transition-all hover:shadow-md"
			>
				<p class="text-[10px] font-bold tracking-widest text-gray-400 uppercase">Tổng doanh thu</p>
				<p
					class="mt-2 origin-left text-3xl font-black text-indigo-600 transition-transform group-hover:scale-105"
				>
					{formatCurrency(vm.stats.totalRevenue)}
				</p>
			</div>
			<div
				class="group rounded-2xl border border-gray-100 bg-white p-6 shadow-sm transition-all hover:shadow-md"
			>
				<p class="text-[10px] font-bold tracking-widest text-gray-400 uppercase">Tổng thuế VAT</p>
				<p
					class="mt-2 origin-left text-3xl font-black text-emerald-600 transition-transform group-hover:scale-105"
				>
					{formatCurrency(vm.stats.totalVat || 0)}
				</p>
			</div>
			<div
				class="group rounded-2xl border border-gray-100 bg-white p-6 shadow-sm transition-all hover:shadow-md"
			>
				<p class="text-[10px] font-bold tracking-widest text-gray-400 uppercase">Tổng đơn hàng</p>
				<p
					class="mt-2 origin-left text-3xl font-black text-orange-600 transition-transform group-hover:scale-105"
				>
					{vm.stats.totalOrders}
				</p>
			</div>
		</div>

		<!-- Charts Grid -->
		<div class="grid grid-cols-1 gap-6 lg:grid-cols-2">
			<!-- Revenue Chart -->
			<div
				class="rounded-2xl border border-gray-100 bg-white p-6 shadow-sm transition-all hover:shadow-md"
			>
				<h3 class="mb-6 text-xs font-bold tracking-widest text-gray-400 uppercase">
					Xu hướng doanh thu
				</h3>
				<div bind:this={revenueChartDiv} class="h-72"></div>
			</div>

			<!-- Orders Chart -->
			<div
				class="rounded-2xl border border-gray-100 bg-white p-6 shadow-sm transition-all hover:shadow-md"
			>
				<h3 class="mb-6 text-xs font-bold tracking-widest text-gray-400 uppercase">
					Số lượng đơn hàng
				</h3>
				<div bind:this={ordersChartDiv} class="h-72"></div>
			</div>
		</div>

		<!-- Service Analysis Section -->
		<div class="grid grid-cols-1 gap-6 lg:grid-cols-3">
			<!-- Service Type Chart -->
			<div
				class="rounded-2xl border border-gray-100 bg-white p-6 shadow-sm transition-all hover:shadow-md"
			>
				<h3 class="mb-6 text-xs font-bold tracking-widest text-gray-400 uppercase">
					Phân bố Nguồn đơn
				</h3>
				<div bind:this={sourceChartDiv} class="h-72"></div>
			</div>

			<!-- Strategic Insight -->
			<div
				class="group relative overflow-hidden rounded-2xl border border-indigo-100 bg-indigo-50/30 p-8 shadow-sm lg:col-span-2"
			>
				<div
					class="absolute -top-16 -right-16 h-48 w-48 rounded-full bg-indigo-100/50 blur-3xl transition-transform group-hover:scale-125"
				></div>

				<div class="relative flex flex-col gap-6 md:flex-row">
					<div
						class="flex h-14 w-14 shrink-0 items-center justify-center rounded-2xl bg-indigo-600 shadow-xl shadow-indigo-200"
					>
						<Icon name="tabler:bulb" class="h-7 w-7 text-white" />
					</div>
					<div>
						<h3 class="text-xl font-black text-indigo-900">AI Strategic Insight</h3>
						<div
							class="prose prose-sm mt-4 max-w-none leading-relaxed text-indigo-900/80 prose-indigo"
						>
							{#if vm.forecast?.recommendation}
								<p class="text-base font-medium">
									{vm.forecast.recommendation}
								</p>
							{:else}
								<div class="flex items-center gap-3 py-2 text-indigo-400 italic">
									<div
										class="h-4 w-4 animate-spin rounded-full border-2 border-indigo-400 border-t-transparent"
									></div>
									Hệ thống đang phân tích xu hướng...
								</div>
							{/if}
						</div>
						<button
							class="mt-6 flex items-center gap-2 text-sm font-extrabold text-indigo-600 transition-colors hover:text-indigo-800"
						>
							<span>Khám phá báo cáo chi tiết</span>
							<Icon name="tabler:arrow-right" class="h-4 w-4" />
						</button>
					</div>
				</div>
			</div>
		</div>

		<!-- AI Forecast Section -->
		<div
			class="rounded-xl border border-indigo-100 bg-gradient-to-br from-indigo-50 to-white p-6 shadow-sm"
		>
			<div class="mb-6 flex items-center justify-between">
				<div class="flex items-center gap-3">
					<div class="rounded-lg bg-indigo-600 p-2">
						<Icon name="tabler:wand" class="h-6 w-6 text-white" />
					</div>
					<div>
						<h3 class="text-lg font-bold text-gray-900">Dự báo doanh thu (AI Powered)</h3>
						<p class="text-sm text-gray-600">
							Sử dụng Machine Learning để dự đoán doanh thu 7 ngày tới
						</p>
					</div>
				</div>
				<div
					class="rounded border border-indigo-200 bg-white px-2 py-1 font-mono text-xs text-indigo-700"
				>
					Model: SSA-Forecasting
				</div>
			</div>

			<div class="rounded-lg border border-indigo-50 bg-white p-4 shadow-sm">
				<div bind:this={forecastChartDiv} class="h-80"></div>
			</div>

			<div class="mt-4 text-center text-sm text-gray-500 italic">
				* Dự báo chỉ mang tính chất tham khảo dựa trên dữ liệu lịch sử.
			</div>
		</div>

		<!-- Top Items Table -->
		<div class="overflow-hidden rounded-xl border border-gray-100 bg-white shadow-sm">
			<div class="border-b border-gray-100 px-6 py-4">
				<h3 class="text-lg font-semibold text-gray-900">Top món bán chạy (Giai đoạn này)</h3>
			</div>
			<div class="overflow-x-auto">
				<table class="min-w-full divide-y divide-gray-200">
					<thead class="bg-gray-50">
						<tr>
							<th
								class="px-6 py-3 text-left text-xs font-medium tracking-wider text-gray-500 uppercase"
								>Tên món</th
							>
							<th
								class="px-6 py-3 text-right text-xs font-medium tracking-wider text-gray-500 uppercase"
								>Số lượng bán</th
							>
							<th
								class="px-6 py-3 text-right text-xs font-medium tracking-wider text-gray-500 uppercase"
								>Tỷ trọng</th
							>
						</tr>
					</thead>
					<tbody class="divide-y divide-gray-200 bg-white">
						{#each vm.stats.topItems as item (item.name)}
							<tr class="hover:bg-gray-50">
								<td class="px-6 py-4 text-sm font-medium whitespace-nowrap text-gray-900"
									>{item.name}</td
								>
								<td class="px-6 py-4 text-right text-sm font-bold whitespace-nowrap text-gray-900"
									>{item.sold}</td
								>
								<td class="px-6 py-4 text-right text-sm whitespace-nowrap text-gray-500">
									<div
										class="mr-2 ml-auto inline-block h-2.5 w-24 rounded-full bg-gray-200 align-middle"
									>
										<div
											class="h-2.5 rounded-full bg-indigo-600"
											style="width: {(item.sold /
												// eslint-disable-next-line @typescript-eslint/no-explicit-any
												Math.max(...vm.stats.topItems.map((i: any) => i.sold))) *
												100}%"
										></div>
									</div>
								</td>
							</tr>
						{/each}
					</tbody>
				</table>
			</div>
		</div>

		<!-- Popular Combos Table -->
		<div class="overflow-hidden rounded-xl border border-gray-100 bg-white shadow-sm">
			<div class="border-b border-gray-100 px-6 py-4">
				<h3 class="text-lg font-semibold text-gray-900">Combo bán chạy (Giai đoạn này)</h3>
			</div>
			<div class="overflow-x-auto">
				<table class="min-w-full divide-y divide-gray-200">
					<thead class="bg-gray-50">
						<tr>
							<th
								class="px-6 py-3 text-left text-xs font-medium tracking-wider text-gray-500 uppercase"
								>Tên Combo</th
							>
							<th
								class="px-6 py-3 text-right text-xs font-medium tracking-wider text-gray-500 uppercase"
								>Số lượng bán</th
							>
							<th
								class="px-6 py-3 text-right text-xs font-medium tracking-wider text-gray-500 uppercase"
								>Tỷ trọng</th
							>
						</tr>
					</thead>
					<tbody class="divide-y divide-gray-200 bg-white">
						{#if vm.stats.popularCombos && vm.stats.popularCombos.length > 0}
							{#each vm.stats.popularCombos as item (item.name)}
								<tr class="hover:bg-gray-50">
									<td class="px-6 py-4 text-sm font-medium whitespace-nowrap text-gray-900"
										>{item.name}</td
									>
									<td class="px-6 py-4 text-right text-sm font-bold whitespace-nowrap text-gray-900"
										>{item.sold}</td
									>
									<td class="px-6 py-4 text-right text-sm whitespace-nowrap text-gray-500">
										<div
											class="mr-2 ml-auto inline-block h-2.5 w-24 rounded-full bg-gray-200 align-middle"
										>
											<div
												class="h-2.5 rounded-full bg-purple-600"
												style="width: {(item.sold /
													// eslint-disable-next-line @typescript-eslint/no-explicit-any
													Math.max(...vm.stats.popularCombos.map((i: any) => i.sold))) *
													100}%"
											></div>
										</div>
									</td>
								</tr>
							{/each}
						{:else}
							<tr>
								<td colspan="3" class="px-6 py-4 text-center text-sm text-gray-500"
									>Chưa có dữ liệu combo</td
								>
							</tr>
						{/if}
					</tbody>
				</table>
			</div>
		</div>
		<!-- AI Combo Recommendations -->
		<div
			class="rounded-xl border border-purple-100 bg-gradient-to-br from-purple-50 to-white p-6 shadow-sm"
		>
			<div class="mb-6 flex items-center justify-between">
				<div class="flex items-center gap-3">
					<div class="rounded-lg bg-purple-600 p-2">
						<Icon name="tabler:wand" class="h-6 w-6 text-white" />
					</div>
					<div>
						<h3 class="text-lg font-bold text-gray-900">Gợi ý Combo (AI Recommendation)</h3>
						<p class="text-sm text-gray-600">
							Các món thường được gọi cùng nhau - Cơ hội tạo Combo mới
						</p>
					</div>
				</div>
			</div>

			{#if vm.recommendations && vm.recommendations.length > 0}
				<div class="grid grid-cols-1 gap-4 md:grid-cols-2 lg:grid-cols-3">
					{#each vm.recommendations as rec, i (i)}
						<div
							class="rounded-lg border border-purple-100 bg-white p-4 shadow-sm transition-shadow hover:shadow-md"
						>
							<div class="mb-3 flex items-center gap-2">
								<span class="rounded bg-gray-100 px-2 py-1 text-xs font-medium text-gray-700"
									>{rec.item1Name}</span
								>
								<span class="text-gray-400">+</span>
								<span class="rounded bg-gray-100 px-2 py-1 text-xs font-medium text-gray-700"
									>{rec.item2Name}</span
								>
							</div>
							<div class="mb-2 flex items-end justify-between">
								<div>
									<p class="text-xs text-gray-500 line-through">
										{formatCurrency(rec.totalOriginalPrice)}
									</p>
									<p class="text-lg font-bold text-purple-600">
										{formatCurrency(rec.suggestedPrice)}
									</p>
								</div>
								<div class="text-right">
									<span
										class="rounded-full bg-green-50 px-2 py-1 text-xs font-medium text-green-600"
										>Tiết kiệm 10%</span
									>
								</div>
							</div>
							<p class="mt-2 border-t border-gray-50 pt-2 text-xs text-gray-500">
								{rec.reason}
							</p>
						</div>
					{/each}
				</div>
			{:else}
				<p class="py-8 text-center text-gray-500">Chưa đủ dữ liệu để gợi ý combo.</p>
			{/if}
		</div>
	{/if}
</div>
