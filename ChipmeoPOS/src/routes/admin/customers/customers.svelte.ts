import { customerAPI, type Customer } from '$lib/api/customer.js';

export class CustomersState {
	customers = $state<Customer[]>([]);
	loading = $state(true);
	searchTerm = $state('');

	get filteredCustomers() {
		return this.customers.filter(
			(c) =>
				c.fullName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
				c.email.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
				(c.phone && c.phone.includes(this.searchTerm))
		);
	}

	async init() {
		await this.loadCustomers();
	}

	async loadCustomers() {
		this.loading = true;
		try {
			this.customers = await customerAPI.getAllCustomers();
		} catch (err) {
			console.error('Failed to load customers:', err);
		} finally {
			this.loading = false;
		}
	}
}
