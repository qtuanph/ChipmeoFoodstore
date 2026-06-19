import { apiClient } from "@/lib/api-client";
import type { Analytics, DashboardStats, Forecast, Recommendation } from "@/lib/types";
import { authHeaders, type ApiResponse } from "./utils";

export const dashboardService = {
  async getStats(): Promise<DashboardStats> {
    const res = await apiClient<ApiResponse<DashboardStats>>("/admin/dashboard/stats", authHeaders());
    return res.data;
  },
  async getAnalytics(fromDate?: string, toDate?: string, groupBy: "day" | "month" = "day"): Promise<Analytics> {
    const params = new URLSearchParams();
    if (fromDate) params.set("fromDate", fromDate);
    if (toDate) params.set("toDate", toDate);
    params.set("groupBy", groupBy);
    const res = await apiClient<ApiResponse<Analytics>>(`/admin/dashboard/analytics?${params}`, authHeaders());
    return res.data;
  },
  async getForecast(horizon: number = 30): Promise<Forecast> {
    const res = await apiClient<ApiResponse<Forecast>>(`/admin/dashboard/forecast?horizon=${horizon}`, authHeaders());
    return res.data;
  },
  async getRecommendations(): Promise<Recommendation[]> {
    const res = await apiClient<ApiResponse<Recommendation[]>>("/admin/dashboard/recommendations", authHeaders());
    return res.data;
  },
};
