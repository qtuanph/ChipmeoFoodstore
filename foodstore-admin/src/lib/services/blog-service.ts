import { apiClient } from "@/lib/api-client";
import type { BlogPost, CreateBlogPostDto, UpdateBlogPostDto, BlogPostRevisionDto, BlogPostBlockDto, CmsDashboardStats } from "@/lib/types";
import { authHeaders, type ApiResponse } from "./utils";

export const blogService = {
  async getAll(status?: string, categorySlug?: string, tagSlug?: string): Promise<BlogPost[]> {
    const params = new URLSearchParams();
    if (status) params.set("status", status);
    if (categorySlug) params.set("categorySlug", categorySlug);
    if (tagSlug) params.set("tagSlug", tagSlug);
    const qs = params.toString();
    const res = await apiClient<ApiResponse<BlogPost[]>>(`/blog${qs ? `?${qs}` : ""}`, authHeaders());
    return res.data;
  },
  async getBySlug(slug: string): Promise<BlogPost> {
    const res = await apiClient<ApiResponse<BlogPost>>(`/blog/slug/${slug}`, authHeaders());
    return res.data;
  },
  async getById(id: string): Promise<BlogPost> {
    const res = await apiClient<ApiResponse<BlogPost>>(`/blog/${id}`, authHeaders());
    return res.data;
  },
  async create(dto: CreateBlogPostDto): Promise<BlogPost> {
    const res = await apiClient<ApiResponse<BlogPost>>("/blog", {
      method: "POST",
      body: JSON.stringify(dto),
      ...authHeaders(),
    });
    return res.data;
  },
  async update(id: string, dto: UpdateBlogPostDto): Promise<BlogPost> {
    const res = await apiClient<ApiResponse<BlogPost>>(`/blog/${id}`, {
      method: "PUT",
      body: JSON.stringify(dto),
      ...authHeaders(),
    });
    return res.data;
  },
  async delete(id: string): Promise<void> {
    await apiClient(`/blog/${id}`, { method: "DELETE", ...authHeaders() });
  },
  async changeStatus(id: string, status: string): Promise<BlogPost> {
    const res = await apiClient<ApiResponse<BlogPost>>(`/blog/${id}/status`, {
      method: "PATCH",
      body: JSON.stringify(status),
      ...authHeaders(),
    });
    return res.data;
  },
  async schedule(id: string, scheduledAt: string | null): Promise<void> {
    await apiClient(`/blog/${id}/schedule`, {
      method: "PATCH",
      body: JSON.stringify(scheduledAt),
      ...authHeaders(),
    });
  },
  async getDashboardStats(): Promise<CmsDashboardStats> {
    const res = await apiClient<ApiResponse<CmsDashboardStats>>("/blog/dashboard/stats", authHeaders());
    return res.data;
  },
  async getFeatured(limit = 5): Promise<BlogPost[]> {
    const res = await apiClient<ApiResponse<BlogPost[]>>(`/blog/featured?limit=${limit}`, authHeaders());
    return res.data;
  },
  async getRevisions(postId: string): Promise<BlogPostRevisionDto[]> {
    const res = await apiClient<ApiResponse<BlogPostRevisionDto[]>>(`/blog/${postId}/revisions`, authHeaders());
    return res.data;
  },
  async createRevision(postId: string): Promise<BlogPostRevisionDto> {
    const res = await apiClient<ApiResponse<BlogPostRevisionDto>>(`/blog/${postId}/revisions`, {
      method: "POST",
      ...authHeaders(),
    });
    return res.data;
  },
  async restoreRevision(revisionId: string): Promise<BlogPost> {
    const res = await apiClient<ApiResponse<BlogPost>>(`/blog/revisions/${revisionId}/restore`, {
      method: "POST",
      ...authHeaders(),
    });
    return res.data;
  },
};