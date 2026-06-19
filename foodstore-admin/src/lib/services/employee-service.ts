import { apiClient } from "@/lib/api-client";
import type { Employee, EmployeeCreateDto, EmployeeUpdateDto } from "@/lib/types";
import { authHeaders, type ApiResponse } from "./utils";

export const employeeService = {
  async getAll(): Promise<Employee[]> {
    const res = await apiClient<ApiResponse<Employee[]>>("/admin/employees", authHeaders());
    return res.data;
  },
  async getById(id: string): Promise<Employee> {
    const res = await apiClient<ApiResponse<Employee>>(`/admin/employees/${id}`, authHeaders());
    return res.data;
  },
  async create(data: EmployeeCreateDto): Promise<Employee> {
    const res = await apiClient<ApiResponse<Employee>>("/admin/employees", {
      method: "POST", body: JSON.stringify(data), ...authHeaders(),
    });
    return res.data;
  },
  async update(id: string, data: EmployeeUpdateDto): Promise<void> {
    await apiClient(`/admin/employees/${id}`, {
      method: "PUT", body: JSON.stringify(data), ...authHeaders(),
    });
  },
  async delete(id: string): Promise<void> {
    await apiClient(`/admin/employees/${id}`, {
      method: "DELETE", ...authHeaders(),
    });
  },
};
