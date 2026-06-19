import { apiClient } from "@/lib/api-client";
import type { Source, SourceCreateDto, SourceUpdateDto } from "@/lib/types";
import { authHeaders, type ApiResponse } from "./utils";

export const sourceService = {
  async getAll(): Promise<Source[]> {
    const res = await apiClient<ApiResponse<Source[]>>("/admin/sources", authHeaders());
    return res.data;
  },
  async getById(id: string): Promise<Source> {
    const res = await apiClient<ApiResponse<Source>>(`/admin/sources/${id}`, authHeaders());
    return res.data;
  },
  async create(data: SourceCreateDto): Promise<Source> {
    const res = await apiClient<ApiResponse<Source>>("/admin/sources", {
      method: "POST", body: JSON.stringify(data), ...authHeaders(),
    });
    return res.data;
  },
  async update(id: string, data: SourceUpdateDto): Promise<void> {
    await apiClient(`/admin/sources/${id}`, {
      method: "PUT", body: JSON.stringify(data), ...authHeaders(),
    });
  },
  async delete(id: string): Promise<void> {
    await apiClient(`/admin/sources/${id}`, {
      method: "DELETE", ...authHeaders(),
    });
  },
};
