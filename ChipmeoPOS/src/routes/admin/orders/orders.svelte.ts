import { ordersAPI } from '$lib/api/index.js';
import { signalRService } from '$lib/services/signalr.js';
import type { Order } from '$lib/types/index.js';

export class OrdersState {
	orders = $state<Order[]>([]);
	loading = $state(true);

	// Toast state
	showToast = $state(false);
	toastMessage = $state('');
	toastType = $state<'success' | 'error'>('success');

	// Modals & Selection
	showHistoryModal = $state(false);
	selectedOrder = $state<Order | null>(null);
	showDetailsModal = $state(false);

	showConfirmModal = $state(false);
	selectedOrderIdToUnpaid = $state<number | null>(null);

	// Pagination & Filtering
	page = $state(1);
	pageSize = $state(10);
	totalCount = $state(0);
	totalPages = $state(1);
	fromDate = $state('');
	toDate = $state('');

	constructor() {
		// Bind methods if needed, though arrow functions or class methods usually work fine in Svelte 5 logic
	}

	async init() {
		await this.loadData();

		// Start SignalR
		const token = localStorage.getItem('token');
		if (token) {
			await signalRService.startConnection(token);
			signalRService.on('ReceiveOrderUpdate', this.handleOrderUpdate.bind(this));
		}
	}

	onDestroy() {
		signalRService.off('ReceiveOrderUpdate', this.handleOrderUpdate.bind(this));
		// Note: off might need exact function ref. binding creates new ref.
		// Better to use arrow function property or store bound ref.
		// Actually, let's use a property for the handler to be safe.
	}

	// Handler as property to preserve 'this' and reference
	handleOrderUpdate = () => {
		this.showSuccess('Có đơn hàng mới/cập nhật!');
		this.loadData();
	};

	async loadData() {
		try {
			this.loading = true;
			const res = await ordersAPI.getPaged(
				this.page,
				this.pageSize,
				this.fromDate || undefined,
				this.toDate || undefined
			);
			this.orders = res.items;
			this.totalCount = res.totalCount;
			this.totalPages = res.totalPages;
			// eslint-disable-next-line @typescript-eslint/no-explicit-any
		} catch (error: any) {
			console.error('[AdminOrders] Error loading data:', error);
			this.showError('Lỗi tải dữ liệu: ' + error.message);
		} finally {
			this.loading = false;
		}
	}

	handlePageChange(newPage: number) {
		if (newPage >= 1 && newPage <= this.totalPages) {
			this.page = newPage;
			this.loadData();
		}
	}

	handleFilter() {
		this.page = 1; // Reset to first page when filtering
		this.loadData();
	}

	async updateStatus(id: number, status: string) {
		try {
			await ordersAPI.updateStatus(id, status);
			this.showSuccess('Cập nhật trạng thái thành công!');
			await this.loadData();
		} catch (error: any) {
			this.showError('Lỗi: ' + error.message);
		}
	}

	handleSetUnpaid(id: number) {
		this.selectedOrderIdToUnpaid = id;
		this.showConfirmModal = true;
	}

	async confirmSetUnpaid() {
		if (!this.selectedOrderIdToUnpaid) return;

		try {
			await ordersAPI.setUnpaid(this.selectedOrderIdToUnpaid);
			this.showSuccess('Đã chuyển đơn hàng về trạng thái chưa thanh toán');
			this.showConfirmModal = false;
			await this.loadData();
			// eslint-disable-next-line @typescript-eslint/no-explicit-any
		} catch (error: any) {
			this.showError('Lỗi: ' + error.message);
		}
	}

	openDetails(order: Order) {
		this.selectedOrder = order;
		this.showDetailsModal = true;
	}

	openHistory(order: Order) {
		this.selectedOrder = order;
		this.showHistoryModal = true;
	}

	getStatusBadge(status: string) {
		const styles = {
			pending: 'bg-yellow-100 text-yellow-800',
			confirmed: 'bg-blue-100 text-blue-800',
			preparing: 'bg-purple-100 text-purple-800',
			ready: 'bg-green-100 text-green-800',
			served: 'bg-teal-100 text-teal-800',
			paid: 'bg-gray-100 text-gray-800',
			cancelled: 'bg-red-100 text-red-800'
		};
		return styles[status as keyof typeof styles] || '';
	}

	private showSuccess(message: string) {
		this.toastMessage = message;
		this.toastType = 'success';
		this.showToast = true;
	}

	private showError(message: string) {
		this.toastMessage = message;
		this.toastType = 'error';
		this.showToast = true;
	}
}
