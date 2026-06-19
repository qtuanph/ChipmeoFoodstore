import { apiClient } from "@/lib/api-client";
import { authHeaders, type ApiResponse } from "./utils";

export interface MediaFile {
  id: string;
  fileName: string;
  fileUrl: string;
  folder: string;
  fileType: string;
  fileSize: number | null;
  altText?: string;
  entityType?: string;
  entityId?: string;
  createdAt: string;
}

export const mediaService = {
  async getAll(folder?: string): Promise<MediaFile[]> {
    const params = folder ? `?folder=${encodeURIComponent(folder)}` : "";
    const res = await apiClient<ApiResponse<MediaFile[]>>(`/media${params}`, authHeaders());
    return res.data;
  },
  async upload(file: File, folder: string = "general"): Promise<{ fileUrl: string }> {
    const formData = new FormData();
    formData.append("file", file);
    formData.append("folder", folder);
    const res = await apiClient<ApiResponse<{ fileUrl: string }>>("/media/upload", {
      method: "POST",
      body: formData,
      ...authHeaders(),
    });
    return res.data;
  },
  async delete(id: string): Promise<void> {
    await apiClient(`/media/${id}`, {
      method: "DELETE",
      ...authHeaders(),
    });
  },
};
