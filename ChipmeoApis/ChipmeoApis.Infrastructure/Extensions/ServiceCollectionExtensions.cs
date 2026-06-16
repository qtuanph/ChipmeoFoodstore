using ChipmeoApis.Usecase.Interfaces;
using ChipmeoApis.Infrastructure.Repositories;
using ChipmeoApis.Infrastructure.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChipmeoApis.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<ISourceRepository, SourceRepository>();
        services.AddScoped<IAddonRepository, AddonRepository>();
        services.AddScoped<IDiscountRepository, DiscountRepository>();
        services.AddScoped<IComboRepository, ComboRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IBlogRepository, BlogRepository>();
        services.AddScoped<IMediaRepository, MediaRepository>();
        services.AddScoped<IPaymentSettingRepository, PaymentSettingRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<ITagRepository, TagRepository>();

        services.AddScoped<IMediaService, MediaHandler>();

        return services;
    }
}
