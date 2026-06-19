import { apiClient } from "@/lib/api-client";
import type { Category, CategoryCreateDto, CategoryUpdateDto } from "@/lib/types";
import { authHeaders, type ApiResponse } from "./utils";

export const categoryService = {
  async getAll(): Promise<Category[]> {
    const res = await apiClient<ApiResponse<Category[]>>("/admin/categories", authHeaders());
    return res.data;
  },
  async getById(id: string): Promise<Category> {
    const res = await apiClient<ApiResponse<Category>>(`/admin/categories/${id}`, authHeaders());
    return res.data;
  },
  async create(data: CategoryCreateDto): Promise<Category> {
    const res = await apiClient<ApiResponse<Category>>("/admin/categories", {
      method: "POST", body: JSON.stringify(data), ...authHeaders(),
    });
    return res.data;
  },
  async update(id: string, data: CategoryUpdateDto): Promise<void> {
    await apiClient(`/admin/categories/${id}`, {
      method: "PUT", body: JSON.stringify(data), ...authHeaders(),
    });
  },
  async delete(id: string): Promise<void> {
    await apiClient(`/admin/categories/${id}`, {
      method: "DELETE", ...authHeaders(),
    });
  },
};
