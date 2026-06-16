using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Core.Entities;
using ChipmeoApis.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ChipmeoApis.Infrastructure.Repositories;

public class PermissionRepository(StoreDbContext context) : IPermissionRepository
{
    private readonly StoreDbContext _context = context;

    public async Task<IEnumerable<Permission>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Permissions.ToListAsync(cancellationToken);
    }

    public async Task<Permission?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Permissions.FindAsync(new object[] { id }, cancellationToken);
    }
}




