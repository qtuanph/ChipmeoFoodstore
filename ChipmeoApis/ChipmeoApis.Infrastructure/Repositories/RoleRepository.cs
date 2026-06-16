using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Core.Entities;
using ChipmeoApis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChipmeoApis.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly StoreDbContext _context;

    public RoleRepository(StoreDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Role>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .ToListAsync(cancellationToken);
    }

    public async Task<Role?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<Role> CreateAsync(Role role, CancellationToken cancellationToken = default)
    {
        _context.Roles.Add(role);
        await _context.SaveChangesAsync(cancellationToken);
        return role;
    }

    public async Task<bool> UpdateAsync(Role role, CancellationToken cancellationToken = default)
    {
        _context.Roles.Update(role);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles.FindAsync(new object[] { id }, cancellationToken);
        if (role == null) return false;
        
        _context.Roles.Remove(role);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<IEnumerable<RolePermission>> GetRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default)
    {
        return await _context.RolePermissions
            .Include(rp => rp.Permission)
            .Where(rp => rp.RoleId == roleId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddRolePermissionsAsync(IEnumerable<RolePermission> permissions, CancellationToken cancellationToken = default)
    {
        await _context.RolePermissions.AddRangeAsync(permissions, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRolePermissionsAsync(IEnumerable<RolePermission> permissions, CancellationToken cancellationToken = default)
    {
        _context.RolePermissions.UpdateRange(permissions);
        await _context.SaveChangesAsync(cancellationToken);
    }
}




