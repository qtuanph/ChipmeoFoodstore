import { apiClient } from "@/lib/api-client";
import type { Role, RoleCreateDto, RoleUpdateDto } from "@/lib/types";
import { authHeaders, type ApiResponse } from "./utils";

export const roleService = {
  async getAll(): Promise<Role[]> {
    const res = await apiClient<ApiResponse<Role[]>>("/admin/roles", authHeaders());
    return res.data;
  },
  async getById(id: string): Promise<Role> {
    const res = await apiClient<ApiResponse<Role>>(`/admin/roles/${id}`, authHeaders());
    return res.data;
  },
  async create(data: RoleCreateDto): Promise<Role> {
    const res = await apiClient<ApiResponse<Role>>("/admin/roles", {
      method: "POST",
      body: JSON.stringify(data),
      ...authHeaders(),
    });
    return res.data;
  },
  async update(id: string, data: RoleUpdateDto): Promise<void> {
    await apiClient(`/admin/roles/${id}`, {
      method: "PUT",
      body: JSON.stringify(data),
      ...authHeaders(),
    });
  },
  async delete(id: string): Promise<void> {
    await apiClient(`/admin/roles/${id}`, {
      method: "DELETE",
      ...authHeaders(),
    });
  },
};
