// Report types
export interface DashboardStats {
	today: { revenue: number; orders: number };
	month: { revenue: number; orders: number };
	total: { revenue: number; orders: number };
	popularItems: PopularItem[];
	popularCombos: PopularCombo[];
	last7Days: DailyStats[];
	last6Months: MonthlyStats[];
	averageOrderValue: number;
	peakHour: string;
	paymentMethodBreakdown: Record<string, number>;
	totalCustomers: number;
	serviceTypeStats: PopularItem[];
}

export interface PopularItem {
	name: string;
	quantity: number;
	revenue: number;
}

export interface PopularCombo {
	name: string;
	quantity: number;
	revenue: number;
}

export interface DailyStats {
	date: string;
	revenue: number;
	orders: number;
}

export interface MonthlyStats {
	month: string;
	revenue: number;
	orders: number;
}

export interface Analytics {
	totalRevenue: number;
	totalVat: number;
	totalOrders: number;
	revenueChart: ChartData[];
	ordersChart: ChartData[];
	topItems: TopItem[];
	popularCombos: PopularCombo[];
}

export interface ChartData {
	label: string;
	value: number;
}

export interface TopItem {
	name: string;
	sold: number;
}

export interface Forecast {
	forecasts: ForecastData[];
}

export interface ForecastData {
	date: string;
	revenue: number;
}

export interface Recommendation {
	item1Name: string;
	item2Name: string;
	frequency: number;
	totalOriginalPrice: number;
	suggestedPrice: number;
	reason: string;
}
