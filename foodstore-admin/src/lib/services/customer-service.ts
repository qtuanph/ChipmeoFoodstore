import { apiClient } from "@/lib/api-client";
import type { Customer, CreateCustomerDto, UpdateCustomerAdminDto } from "@/lib/types";
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
};
