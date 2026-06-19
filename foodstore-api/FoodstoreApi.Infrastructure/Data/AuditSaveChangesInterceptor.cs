using FoodstoreApi.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace FoodstoreApi.Infrastructure.Data;

public class AuditSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditSaveChangesInterceptor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null) return base.SavingChangesAsync(eventData, result, cancellationToken);

        var now = DateTime.UtcNow;
        var userId = GetCurrentUserId();

        foreach (var entry in context.ChangeTracker.Entries<IAuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
                entry.Entity.UpdatedAt = now;
                if (userId.HasValue)
                {
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.UpdatedBy = userId;
                }
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = now;
                if (userId.HasValue)
                    entry.Entity.UpdatedBy = userId;
            }
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private Guid? GetCurrentUserId()
    {
        try
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var claim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (claim != null && Guid.TryParse(claim.Value, out var id))
                    return id;
            }
        }
        catch
        {
            // Ignore — no HTTP context (e.g., background jobs, seeder)
        }
        return null;
    }
}