using FoodstoreApi.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodstoreApi.Infrastructure.Data;

public static class AuditableEntityExtensions
{
    public static EntityTypeBuilder<T> ConfigureAudit<T>(this EntityTypeBuilder<T> builder)
        where T : class, IAuditableEntity
    {
        builder.Property(e => e.CreatedAt).HasColumnName("created_at");
        builder.Property(e => e.UpdatedAt).HasColumnName("updated_at");
        builder.Property(e => e.CreatedBy).HasColumnName("created_by");
        builder.Property(e => e.UpdatedBy).HasColumnName("updated_by");
        return builder;
    }
}