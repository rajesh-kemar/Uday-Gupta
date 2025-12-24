using Kemar.WHL.Repository.Interfaces;
using Kemar.WHL.Repository.Repositories;

namespace Kemar.WHL.API.Extensions
{
    public static class RepositoryRegistration
    {
        public static IServiceCollection AddRepositoryLayer(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IWarehouseRepository, WarehouseRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IShipmentRepository, ShipmentRepository>();
            services.AddScoped<IDistanceRepository, DistanceRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();

            return services;
        }
    }
}