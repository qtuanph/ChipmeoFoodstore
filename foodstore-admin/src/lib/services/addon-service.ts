import { apiClient } from "@/lib/api-client";
import type { Addon, AddonCreateDto, AddonUpdateDto } from "@/lib/types";
import { authHeaders, type ApiResponse } from "./utils";

export const addonService = {
  async getAll(): Promise<Addon[]> {
    const res = await apiClient<ApiResponse<Addon[]>>("/admin/addons", authHeaders());
    return res.data;
  },
  async getById(id: string): Promise<Addon> {
    const res = await apiClient<ApiResponse<Addon>>(`/admin/addons/${id}`, authHeaders());
    return res.data;
  },
  async create(data: AddonCreateDto): Promise<Addon> {
    const res = await apiClient<ApiResponse<Addon>>("/admin/addons", {
      method: "POST", body: JSON.stringify(data), ...authHeaders(),
    });
    return res.data;
  },
  async update(id: string, data: AddonUpdateDto): Promise<void> {
    await apiClient(`/admin/addons/${id}`, {
      method: "PUT", body: JSON.stringify(data), ...authHeaders(),
    });
  },
  async delete(id: string): Promise<void> {
    await apiClient(`/admin/addons/${id}`, {
      method: "DELETE", ...authHeaders(),
    });
  },
};
