import { apiClient } from "@/lib/api-client";
import type { Combo, ComboCreateDto, ComboUpdateDto } from "@/lib/types";
import { authHeaders, type ApiResponse } from "./utils";

export const comboService = {
  async getAll(): Promise<Combo[]> {
    const res = await apiClient<ApiResponse<Combo[]>>("/admin/combos", authHeaders());
    return res.data;
  },
  async getById(id: string): Promise<Combo> {
    const res = await apiClient<ApiResponse<Combo>>(`/admin/combos/${id}`, authHeaders());
    return res.data;
  },
  async create(data: ComboCreateDto): Promise<Combo> {
    const res = await apiClient<ApiResponse<Combo>>("/admin/combos", {
      method: "POST", body: JSON.stringify(data), ...authHeaders(),
    });
    return res.data;
  },
  async update(id: string, data: ComboUpdateDto): Promise<void> {
    await apiClient(`/admin/combos/${id}`, {
      method: "PUT", body: JSON.stringify(data), ...authHeaders(),
    });
  },
  async delete(id: string): Promise<void> {
    await apiClient(`/admin/combos/${id}`, {
      method: "DELETE", ...authHeaders(),
    });
  },
};
