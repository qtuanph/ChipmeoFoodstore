import type { AuthUser } from "./auth-service";

export function hasRole(user: AuthUser | null, role: string): boolean {
  return user?.roleName === role;
}

export function hasPermission(user: AuthUser | null, permission: string): boolean {
  return user?.permissions.includes(permission) ?? false;
}

export function hasAnyPermission(user: AuthUser | null, permissions: string[]): boolean {
  return permissions.some((p) => hasPermission(user, p));
}

export function hasAllPermissions(user: AuthUser | null, permissions: string[]): boolean {
  return permissions.every((p) => hasPermission(user, p));
}
