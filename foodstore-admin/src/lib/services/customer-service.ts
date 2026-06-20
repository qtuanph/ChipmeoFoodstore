import { apiClient } from "@/lib/api-client";
import type { Customer, CreateCustomerDto, UpdateCustomerAdminDto, AddPointsDto, CustomerOrderHistory, UpcomingBirthdays } from "@/lib/types";
import { authHeaders, type ApiResponse } from "./utils";

export const customerService = {
  async getAll(): Promise<Customer[]> {
    const res = await apiClient<ApiResponse<Customer[]>>("/admin/customers", authHeaders());
    return res.data;
  },
  async getById(id: string): Promise<Customer> {
    const res = await apiClient<ApiResponse<Customer>>(`/admin/customers/${id}`, authHeaders());
    return res.data;
  },
  async create(dto: CreateCustomerDto): Promise<Customer> {
    const res = await apiClient<ApiResponse<Customer>>("/admin/customers", {
      method: "POST",
      body: JSON.stringify(dto),
      ...authHeaders(),
    });
    return res.data;
  },
  async update(id: string, dto: UpdateCustomerAdminDto): Promise<void> {
    await apiClient(`/admin/customers/${id}`, {
      method: "PUT",
      body: JSON.stringify(dto),
      ...authHeaders(),
    });
  },
  async delete(id: string): Promise<void> {
    await apiClient(`/admin/customers/${id}`, {
      method: "DELETE",
      ...authHeaders(),
    });
  },
  async addPoints(id: string, dto: AddPointsDto): Promise<Customer> {
    const res = await apiClient<ApiResponse<Customer>>(`/admin/customers/${id}/add-points`, {
      method: "POST",
      body: JSON.stringify(dto),
      ...authHeaders(),
    });
    return res.data;
  },
  async getOrderHistory(id: string): Promise<CustomerOrderHistory[]> {
    const res = await apiClient<ApiResponse<CustomerOrderHistory[]>>(`/admin/customers/${id}/orders`, authHeaders());
    return res.data;
  },
  async getUpcomingBirthdays(): Promise<UpcomingBirthdays> {
    const res = await apiClient<ApiResponse<UpcomingBirthdays>>("/admin/customers/birthdays", authHeaders());
    return res.data;
  },
};
