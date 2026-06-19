import { apiClient } from "@/lib/api-client";
import type { TagDto, CreateTagDto, UpdateTagDto } from "@/lib/types";
import { authHeaders, type ApiResponse } from "./utils";

export const blogTagService = {
  async getAll(): Promise<TagDto[]> {
    const res = await apiClient<ApiResponse<TagDto[]>>("/tags", authHeaders());
    return res.data;
  },
  async getById(id: string): Promise<TagDto> {
    const res = await apiClient<ApiResponse<TagDto>>(`/tags/${id}`, authHeaders());
    return res.data;
  },
  async create(dto: CreateTagDto): Promise<TagDto> {
    const res = await apiClient<ApiResponse<TagDto>>("/tags", {
      method: "POST",
      body: JSON.stringify(dto),
      ...authHeaders(),
    });
    return res.data;
  },
  async update(id: string, dto: UpdateTagDto): Promise<TagDto> {
    const res = await apiClient<ApiResponse<TagDto>>(`/tags/${id}`, {
      method: "PUT",
      body: JSON.stringify(dto),
      ...authHeaders(),
    });
    return res.data;
  },
  async delete(id: string): Promise<void> {
    await apiClient(`/tags/${id}`, { method: "DELETE", ...authHeaders() });
  },
};