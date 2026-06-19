import { apiClient } from "@/lib/api-client";
import type { MenuItem, MenuItemCreateDto, MenuItemUpdateDto } from "@/lib/types";
import { authHeaders, type ApiResponse } from "./utils";

export const menuItemService = {
  async getAll(): Promise<MenuItem[]> {
    const res = await apiClient<ApiResponse<MenuItem[]>>("/admin/menu-items", authHeaders());
    return res.data;
  },
  async getById(id: string): Promise<MenuItem> {
    const res = await apiClient<ApiResponse<MenuItem>>(`/admin/menu-items/${id}`, authHeaders());
    return res.data;
  },
  async create(data: MenuItemCreateDto): Promise<MenuItem> {
    const res = await apiClient<ApiResponse<MenuItem>>("/admin/menu-items", {
      method: "POST", body: JSON.stringify(data), ...authHeaders(),
    });
    return res.data;
  },
  async update(id: string, data: MenuItemUpdateDto): Promise<void> {
    await apiClient(`/admin/menu-items/${id}`, {
      method: "PUT", body: JSON.stringify(data), ...authHeaders(),
    });
  },
  async delete(id: string): Promise<void> {
    await apiClient(`/admin/menu-items/${id}`, {
      method: "DELETE", ...authHeaders(),
    });
  },
};
