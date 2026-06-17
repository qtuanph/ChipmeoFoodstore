import { reportsAPI } from '$lib/api/index.js';
import { formatCurrency } from '$lib/utils/index.js';
import ApexCharts from 'apexcharts';

export class DashboardState {
	stats = $state<any>(null);
	loading = $state(true);
	error = $state<string | null>(null);
	lastUpdate = $state(new Date());

	async loadData() {
		try {
			this.loading = true;
			this.stats = await reportsAPI.getDashboardStats();
			this.lastUpdate = new Date();
		} catch (err: any) {
			console.error('Failed to load dashboard stats:', err);
			this.error = err.message || 'Failed to load statistics';
		} finally {
			this.loading = false;
		}
	}

	handleOrderUpdate(data: any) {
		this.loadData();
	}

	initCharts(containers: {
		revenue?: HTMLElement;
		popularItems?: HTMLElement;
		popularCombos?: HTMLElement;
		paymentMethods?: HTMLElement;
		serviceType?: HTMLElement;
	}) {
		if (!this.stats) return;

		// Revenue Trend Chart (Last 7 Days)
		if (containers.revenue) {
			const options = {
				series: [
					{
						name: 'Doanh thu (7 ngày qua)',
						data: this.stats.last7Days.map((d: any) => d.revenue)
					}
				],
				chart: {
					type: 'area' as const,
					height: 480,
					toolbar: { show: false }
				},
				colors: ['#4f46e5'],
				stroke: { curve: 'smooth' as const, width: 2 },
				fill: {
					type: 'gradient',
					gradient: {
						shadeIntensity: 1,
						opacityFrom: 0.45,
						opacityTo: 0.05
					}
				},
				dataLabels: { enabled: false },
				xaxis: {
					categories: this.stats.last7Days.map((d: any) => {
						const date = new Date(d.date);
						return `${date.getDate()}/${date.getMonth() + 1}`;
					}),
					axisBorder: { show: false },
					axisTicks: { show: false }
				},
				yaxis: {
					labels: {
						formatter: (val: number) => formatCurrency(val)
					}
				},
				tooltip: {
					y: {
						formatter: (val: number) => formatCurrency(val)
					}
				}
			};
			const chart = new ApexCharts(containers.revenue, options);
			containers.revenue.innerHTML = '';
			chart.render();
		}

		// Popular Items Chart
		if (containers.popularItems) {
			const options = {
				series: [
					{
						name: 'Số lượng bán',
						data: this.stats.popularItems.map((i: any) => i.quantity)
					}
				],
				chart: {
					type: 'bar' as const,
					height: 250,
					toolbar: { show: false }
				},
				plotOptions: {
					bar: {
						borderRadius: 4,
						horizontal: true
					}
				},
				colors: ['#36a2eb'],
				dataLabels: { enabled: false },
				xaxis: {
					categories: this.stats.popularItems.map((i: any) => i.name)
				}
			};
			const chart = new ApexCharts(containers.popularItems, options);
			containers.popularItems.innerHTML = '';
			chart.render();
		}

		// Popular Combos Chart
		if (containers.popularCombos) {
			const options = {
				series: [
					{
						name: 'Số lượng bán',
						data: this.stats.popularCombos.map((i: any) => i.quantity)
					}
				],
				chart: {
					type: 'bar' as const,
					height: 250,
					toolbar: { show: false }
				},
				plotOptions: {
					bar: {
						borderRadius: 4,
						horizontal: true
					}
				},
				colors: ['#ff9f40'],
				dataLabels: { enabled: false },
				xaxis: {
					categories: this.stats.popularCombos.map((i: any) => i.name)
				}
			};
			const chart = new ApexCharts(containers.popularCombos, options);
			containers.popularCombos.innerHTML = '';
			chart.render();
		}

		// Payment Methods Chart
		if (containers.paymentMethods && this.stats.paymentMethodBreakdown) {
			const labels = Object.keys(this.stats.paymentMethodBreakdown);
			const data = Object.values(this.stats.paymentMethodBreakdown) as number[];

			const options = {
				series: data,
				chart: {
					type: 'donut' as const,
					height: 200
				},
				labels: labels.map((l) =>
					l === 'cash' ? 'Tiền mặt' : l === 'banking' ? 'Chuyển khoản' : l
				),
				colors: ['#22c55e', '#3b82f6', '#f9712e'],
				stroke: { show: false },
				dataLabels: { enabled: false },
				legend: { position: 'bottom' as const },
				tooltip: {
					y: {
						formatter: (val: number) => formatCurrency(val)
					}
				}
			};
			const chart = new ApexCharts(containers.paymentMethods, options);
			containers.paymentMethods.innerHTML = '';
			chart.render();
		}

		// Order Source (Today) Chart
		if (containers.serviceType && this.stats.serviceTypeStats) {
			const options = {
				series: this.stats.serviceTypeStats.map((i: any) => i.quantity),
				chart: {
					type: 'donut' as const,
					height: 200
				},
				labels: this.stats.serviceTypeStats.map((i: any) => i.name),
				colors: ['#4f46e5', '#ec4899', '#3b82f6', '#10b981', '#f97316'],
				stroke: { show: false },
				dataLabels: { enabled: false },
				legend: { position: 'bottom' as const },
				tooltip: {
					y: {
						formatter: (val: number) => `${val} đơn`
					}
				}
			};
			const chart = new ApexCharts(containers.serviceType, options);
			containers.serviceType.innerHTML = '';
			chart.render();
		}
	}
}
