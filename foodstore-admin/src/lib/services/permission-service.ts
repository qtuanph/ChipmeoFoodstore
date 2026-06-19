import { apiClient } from "@/lib/api-client";
import type { Permission } from "@/lib/types";
import { authHeaders, type ApiResponse } from "./utils";

export const permissionService = {
  async getAll(): Promise<Permission[]> {
    const res = await apiClient<ApiResponse<Record<string, Permission[]>>>("/admin/permissions", authHeaders());
    const grouped = res.data;
    const flat: Permission[] = [];
    for (const [module, perms] of Object.entries(grouped)) {
      for (const p of perms) {
        flat.push({ ...p, module });
      }
    }
    return flat;
  },
  async getRolePermissions(roleId: string): Promise<string[]> {
    const res = await apiClient<ApiResponse<string[]>>(`/admin/roles/${roleId}/permissions`, authHeaders());
    return res.data;
  },
  async updateRolePermissions(roleId: string, permissionCodes: string[]): Promise<void> {
    await apiClient(`/admin/roles/${roleId}/permissions`, {
      method: "PUT",
      body: JSON.stringify(permissionCodes),
      ...authHeaders(),
    });
  },
};
