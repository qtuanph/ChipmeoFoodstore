import { apiClient } from "@/lib/api-client";
import type { Discount, DiscountCreateDto, DiscountUpdateDto } from "@/lib/types";
import { authHeaders, type ApiResponse } from "./utils";

export const discountService = {
  async getAll(): Promise<Discount[]> {
    const res = await apiClient<ApiResponse<Discount[]>>("/admin/discounts", authHeaders());
    return res.data;
  },
  async getById(id: string): Promise<Discount> {
    const res = await apiClient<ApiResponse<Discount>>(`/admin/discounts/${id}`, authHeaders());
    return res.data;
  },
  async create(data: DiscountCreateDto): Promise<Discount> {
    const res = await apiClient<ApiResponse<Discount>>("/admin/discounts", {
      method: "POST", body: JSON.stringify(data), ...authHeaders(),
    });
    return res.data;
  },
  async update(id: string, data: DiscountUpdateDto): Promise<void> {
    await apiClient(`/admin/discounts/${id}`, {
      method: "PUT", body: JSON.stringify(data), ...authHeaders(),
    });
  },
  async delete(id: string): Promise<void> {
    await apiClient(`/admin/discounts/${id}`, {
      method: "DELETE", ...authHeaders(),
    });
  },
};
