import { apiClient } from "@/lib/api-client";
import type { BlogCategoryDto, CreateBlogCategoryDto, UpdateBlogCategoryDto } from "@/lib/types";
import { authHeaders, type ApiResponse } from "./utils";

export const blogCategoryService = {
  async getAll(): Promise<BlogCategoryDto[]> {
    const res = await apiClient<ApiResponse<BlogCategoryDto[]>>("/blog/categories", authHeaders());
    return res.data;
  },
  async getById(id: string): Promise<BlogCategoryDto> {
    const res = await apiClient<ApiResponse<BlogCategoryDto>>(`/blog/categories/${id}`, authHeaders());
    return res.data;
  },
  async create(dto: CreateBlogCategoryDto): Promise<BlogCategoryDto> {
    const res = await apiClient<ApiResponse<BlogCategoryDto>>("/blog/categories", {
      method: "POST",
      body: JSON.stringify(dto),
      ...authHeaders(),
    });
    return res.data;
  },
  async update(id: string, dto: UpdateBlogCategoryDto): Promise<BlogCategoryDto> {
    const res = await apiClient<ApiResponse<BlogCategoryDto>>(`/blog/categories/${id}`, {
      method: "PUT",
      body: JSON.stringify(dto),
      ...authHeaders(),
    });
    return res.data;
  },
  async delete(id: string): Promise<void> {
    await apiClient(`/blog/categories/${id}`, { method: "DELETE", ...authHeaders() });
  },
};