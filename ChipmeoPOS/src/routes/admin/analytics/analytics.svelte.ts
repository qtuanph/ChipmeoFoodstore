import { reportsAPI } from '$lib/api/index.js';
import { formatCurrency } from '$lib/utils/index.js';
import ApexCharts from 'apexcharts';

export class AnalyticsState {
	stats = $state<any>(null);
	forecast = $state<any>(null);
	recommendations = $state<any>(null);
	loading = $state(true);
	error = $state<string | null>(null);
	lastUpdate = $state(new Date());

	// Filter State
	dateRange = $state('7days');
	fromDate = $state('');
	toDate = $state('');
	groupBy = $state<'day' | 'month'>('day');
	forecastHorizon = $state(7);

	updateDatesFromRange(range: string) {
		const end = new Date();
		const start = new Date();

		if (range === '7days') {
			start.setDate(end.getDate() - 7);
			this.groupBy = 'day';
		} else if (range === '30days') {
			start.setDate(end.getDate() - 30);
			this.groupBy = 'day';
		} else if (range === 'thisMonth') {
			start.setDate(1);
			this.groupBy = 'day';
		} else if (range === 'last3Months') {
			start.setMonth(end.getMonth() - 3);
			start.setDate(1);
			this.groupBy = 'month';
		}

		// Format as YYYY-MM-DD using local time
		const formatDate = (date: Date) => {
			const year = date.getFullYear();
			const month = String(date.getMonth() + 1).padStart(2, '0');
			const day = String(date.getDate()).padStart(2, '0');
			return `${year}-${month}-${day}`;
		};

		this.fromDate = formatDate(start);
		this.toDate = formatDate(end);

		// Trigger reload if mounted
		if (!this.loading) this.loadData();
	}

	async loadData() {
		try {
			this.loading = true;
			this.error = null;

			// Load Analytics
			this.stats = await reportsAPI.getAnalytics(this.fromDate, this.toDate, this.groupBy);

			// Load Forecast
			try {
				this.forecast = await reportsAPI.getForecast(this.forecastHorizon);
			} catch (err: any) {
				console.warn('Forecast unavailable:', err.message);
				this.forecast = null;
			}

			// Load Recommendations
			this.recommendations = await reportsAPI.getRecommendations();

			this.lastUpdate = new Date();
		} catch (err: any) {
			console.error('Failed to load analytics:', err);
			this.error = err.message || 'Failed to load analytics';
		} finally {
			this.loading = false;
		}
	}

	handleOrderUpdate(data: any) {
		this.loadData();
	}

	formatDateLabel(dateStr: string, type: 'day' | 'month'): string {
		if (!dateStr) return '';
		try {
			if (type === 'month') {
				const parts = dateStr.split('-');
				if (parts.length >= 2) {
					return `${parts[1]}/${parts[0]}`;
				}
			} else {
				const parts = dateStr.split('-');
				if (parts.length >= 3) {
					return `${parts[2]}/${parts[1]}/${parts[0]}`;
				}
			}
			return dateStr;
		} catch (e) {
			return dateStr;
		}
	}

	initCharts(containers: {
		revenue?: HTMLElement;
		orders?: HTMLElement;
		forecast?: HTMLElement;
		source?: HTMLElement;
	}) {
		if (this.stats && containers.revenue) {
			const options = {
				series: [
					{
						name: 'Doanh thu',
						data: this.stats.revenueChart.map((d: any) => d.value)
					}
				],
				chart: {
					type: 'area',
					height: 280,
					toolbar: { show: false },
					zoom: { enabled: false }
				},
				dataLabels: { enabled: false },
				stroke: { curve: 'smooth', width: 2 },
				fill: {
					type: 'gradient',
					gradient: {
						shadeIntensity: 1,
						opacityFrom: 0.5,
						opacityTo: 0,
						stops: [0, 90, 100]
					}
				},
				colors: ['#4f46e5'],
				xaxis: {
					categories: this.stats.revenueChart.map((d: any) =>
						this.formatDateLabel(d.label, this.groupBy)
					),
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

		if (this.stats && containers.orders) {
			const options = {
				series: [
					{
						name: 'Số đơn hàng',
						data: this.stats.ordersChart.map((d: any) => d.value)
					}
				],
				chart: {
					type: 'bar',
					height: 280,
					toolbar: { show: false }
				},
				plotOptions: {
					bar: {
						borderRadius: 4,
						columnWidth: '60%'
					}
				},
				dataLabels: { enabled: false },
				colors: ['#f97316'],
				xaxis: {
					categories: this.stats.ordersChart.map((d: any) =>
						this.formatDateLabel(d.label, this.groupBy)
					),
					axisBorder: { show: false },
					axisTicks: { show: false }
				},
				yaxis: {
					labels: {
						formatter: (val: number) => val.toString()
					}
				}
			};

			const chart = new ApexCharts(containers.orders, options);
			containers.orders.innerHTML = '';
			chart.render();
		}

		if (this.forecast && containers.forecast) {
			const options = {
				series: [
					{
						name: 'Dự báo Doanh thu (AI)',
						data: this.forecast.forecasts.map((d: any) => d.revenue)
					}
				],
				chart: {
					type: 'line',
					height: 280,
					toolbar: { show: false }
				},
				stroke: {
					curve: 'smooth',
					width: 2,
					dashArray: [5]
				},
				colors: ['#10b981'],
				xaxis: {
					categories: this.forecast.forecasts.map((d: any) => this.formatDateLabel(d.date, 'day')),
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
						formatter: (val: number) => formatCurrency(val) + ' (Dự báo)'
					}
				}
			};

			const chart = new ApexCharts(containers.forecast, options);
			containers.forecast.innerHTML = '';
			chart.render();
		}

		if (this.stats && this.stats.sourceStats && containers.source) {
			const options = {
				series: this.stats.sourceStats.map((d: any) => d.sold),
				chart: {
					type: 'donut',
					height: 280
				},
				labels: this.stats.sourceStats.map((d: any) => d.name),
				colors: ['#4f46e5', '#f97316', '#10b981', '#ec4899', '#6366f1'],
				stroke: { show: false },
				dataLabels: { enabled: false },
				legend: {
					position: 'bottom'
				},
				plotOptions: {
					pie: {
						donut: {
							size: '65%'
						}
					}
				}
			};

			const chart = new ApexCharts(containers.source, options);
			containers.source.innerHTML = '';
			chart.render();
		}
	}
}
