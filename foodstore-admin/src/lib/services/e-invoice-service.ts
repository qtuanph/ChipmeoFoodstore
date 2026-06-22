import { apiClient } from "@/lib/api-client"
import type {
  EInvoiceProvider, CreateEInvoiceProviderDto, UpdateEInvoiceProviderDto,
  EInvoice, IssueInvoiceDto, CancelInvoiceDto,
  EInvoiceSetting, UpdateEInvoiceSettingDto,
  EInvoiceDashboard,
} from "@/lib/types"
import { authHeaders, type ApiResponse } from "./utils"

export const eInvoiceService = {
  // Dashboard
  async getDashboard(): Promise<EInvoiceDashboard> {
    const res = await apiClient<ApiResponse<EInvoiceDashboard>>("/admin/e-invoice/dashboard", authHeaders())
    return res.data
  },

  // Providers
  async getAllProviders(): Promise<EInvoiceProvider[]> {
    const res = await apiClient<ApiResponse<EInvoiceProvider[]>>("/admin/e-invoice/providers", authHeaders())
    return res.data
  },
  async getProviderById(id: string): Promise<EInvoiceProvider> {
    const res = await apiClient<ApiResponse<EInvoiceProvider>>(`/admin/e-invoice/providers/${id}`, authHeaders())
    return res.data
  },
  async createProvider(data: CreateEInvoiceProviderDto): Promise<EInvoiceProvider> {
    const res = await apiClient<ApiResponse<EInvoiceProvider>>("/admin/e-invoice/providers", {
      method: "POST", body: JSON.stringify(data), ...authHeaders(),
    })
    return res.data
  },
  async updateProvider(id: string, data: UpdateEInvoiceProviderDto): Promise<void> {
    await apiClient(`/admin/e-invoice/providers/${id}`, {
      method: "PUT", body: JSON.stringify(data), ...authHeaders(),
    })
  },
  async deleteProvider(id: string): Promise<void> {
    await apiClient(`/admin/e-invoice/providers/${id}`, {
      method: "DELETE", ...authHeaders(),
    })
  },
  async testProviderConnection(id: string): Promise<boolean> {
    const res = await apiClient<ApiResponse<{ connected: boolean }>>(`/admin/e-invoice/providers/${id}/test`, {
      method: "POST", ...authHeaders(),
    })
    return res.data.connected
  },

  // Invoices
  async getAllInvoices(): Promise<EInvoice[]> {
    const res = await apiClient<ApiResponse<EInvoice[]>>("/admin/e-invoice/invoices", authHeaders())
    return res.data
  },
  async getInvoiceById(id: string): Promise<EInvoice> {
    const res = await apiClient<ApiResponse<EInvoice>>(`/admin/e-invoice/invoices/${id}`, authHeaders())
    return res.data
  },
  async issueInvoice(orderId: string, data: IssueInvoiceDto): Promise<EInvoice> {
    const res = await apiClient<ApiResponse<EInvoice>>(`/admin/e-invoice/orders/${orderId}/invoice`, {
      method: "POST", body: JSON.stringify(data), ...authHeaders(),
    })
    return res.data
  },
  async cancelInvoice(id: string, data: CancelInvoiceDto): Promise<EInvoice> {
    const res = await apiClient<ApiResponse<EInvoice>>(`/admin/e-invoice/invoices/${id}/cancel`, {
      method: "POST", body: JSON.stringify(data), ...authHeaders(),
    })
    return res.data
  },

  // Settings
  async getSettings(): Promise<EInvoiceSetting | null> {
    const res = await apiClient<ApiResponse<EInvoiceSetting | Record<string, never>>>("/admin/e-invoice/settings", authHeaders())
    if (!res.data || !("id" in res.data)) return null
    return res.data as EInvoiceSetting
  },
  async updateSettings(data: UpdateEInvoiceSettingDto): Promise<EInvoiceSetting> {
    const res = await apiClient<ApiResponse<EInvoiceSetting>>("/admin/e-invoice/settings", {
      method: "PUT", body: JSON.stringify(data), ...authHeaders(),
    })
    return res.data
  },
}
