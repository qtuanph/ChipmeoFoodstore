import { apiClient } from "@/lib/api-client";
import type { BlogSettingDto, UpdateBlogSettingDto } from "@/lib/types";
import { authHeaders, type ApiResponse } from "./utils";

export const blogSettingService = {
  async getAll(): Promise<BlogSettingDto[]> {
    const res = await apiClient<ApiResponse<BlogSettingDto[]>>("/blog/settings", authHeaders());
    return res.data;
  },
  async getByKey(key: string): Promise<BlogSettingDto> {
    const res = await apiClient<ApiResponse<BlogSettingDto>>(`/blog/settings/${key}`, authHeaders());
    return res.data;
  },
  async upsert(key: string, dto: UpdateBlogSettingDto): Promise<BlogSettingDto> {
    const res = await apiClient<ApiResponse<BlogSettingDto>>(`/blog/settings/${key}`, {
      method: "PUT",
      body: JSON.stringify(dto),
      ...authHeaders(),
    });
    return res.data;
  },
  async delete(id: string): Promise<void> {
    await apiClient(`/blog/settings/${id}`, { method: "DELETE", ...authHeaders() });
  },
};