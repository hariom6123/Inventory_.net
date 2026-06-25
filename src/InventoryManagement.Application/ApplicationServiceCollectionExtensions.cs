using AutoMapper;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Application.Mappings;
using InventoryManagement.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagement.Application
{
    /// <summary>
    /// Extension methods for registering Application-layer services.
    /// </summary>
    public static class ApplicationServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all Application-layer services into the supplied <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection to add registrations to.</param>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<DashboardService>();
            return services;
        }
    }
}