"use client";

import { createContext, useContext, useState, useCallback, useEffect, type ReactNode } from "react";
import { useRouter } from "next/navigation";
import { authService, type AuthUser } from "./auth-service";

interface AuthContextType {
  user: AuthUser | null;
  isLoading: boolean;
  login: (username: string, password: string) => Promise<void>;
  logout: () => Promise<void>;
  updateProfile: (dto: { name?: string; email?: string; phone?: string; avatarUrl?: string }) => Promise<void>;
  isAuthenticated: boolean;
  hasRole: (role: string) => boolean;
  hasPermission: (permission: string) => boolean;
}

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const router = useRouter();
  const [user, setUser] = useState<AuthUser | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    authService
      .getProfile()
      .then((u) => setUser(u))
      .catch(() => setUser(null))
      .finally(() => setIsLoading(false));
  }, []);

  const login = useCallback(async (username: string, password: string) => {
    const result = await authService.login(username, password);
    setUser(result.user);
  }, []);

  const logout = useCallback(async () => {
    await authService.logout();
    setUser(null);
    router.push("/login");
  }, [router]);

  const updateProfile = useCallback(async (dto: {
    name?: string; email?: string; phone?: string; avatarUrl?: string;
  }) => {
    const updated = await authService.updateProfile(dto);
    setUser(updated);
  }, []);

  const isAuthenticated = user !== null;

  const hasRole = useCallback(
    (role: string) => isAuthenticated && user!.roleName === role,
    [isAuthenticated, user],
  );

  const hasPermission = useCallback(
    (permission: string) => isAuthenticated && user!.permissions.includes(permission),
    [isAuthenticated, user],
  );

  return (
    <AuthContext.Provider value={{ user, isLoading, login, logout, updateProfile, isAuthenticated, hasRole, hasPermission }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth(): AuthContextType {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within an AuthProvider");
  return ctx;
}
