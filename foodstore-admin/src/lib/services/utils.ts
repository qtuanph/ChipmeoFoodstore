import { getAuthToken } from "@/lib/auth-service";

export function authHeaders(): { token: string | undefined } {
  const token = getAuthToken();
  return { token: token ?? undefined };
}

export interface ApiResponse<T> {
  data: T;
}
