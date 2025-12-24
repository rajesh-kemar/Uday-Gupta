using AutoMapper;
using Kemar.WHL.Model.Request;
using Kemar.WHL.Model.Response;
using Kemar.WHL.Repository.Entities;
using Kemar.WHL.Repository.Models.User;

namespace Kemar.WHL.API.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {

            CreateMap<ProductRequest, Product>().ReverseMap();
            CreateMap<Product, ProductResponse>().ReverseMap();

            CreateMap<InventoryRequest, Inventory>().ReverseMap();
            CreateMap<Inventory, InventoryResponse>().ReverseMap();

            CreateMap<WarehouseRequest, Warehouse>().ReverseMap();
            CreateMap<Warehouse, WarehouseResponse>().ReverseMap();

            CreateMap<VehicleRequest, Vehicle>().ReverseMap();
            CreateMap<Vehicle, VehicleResponse>().ReverseMap();

            CreateMap<ShipmentRequest, Shipment>().ReverseMap();
            CreateMap<Shipment, ShipmentResponse>().ReverseMap();

            CreateMap<DistanceRequest, Distance>().ReverseMap();
            CreateMap<Distance, DistanceResponse>().ReverseMap();

            CreateMap<PickingRequest, Picking>();
            CreateMap<Picking, PickingResponse>();

            CreateMap<UserRequest, User>()
                .ForMember(dest => dest.PasswordHash,
                    opt => opt.MapFrom(src =>
                        System.Text.Encoding.UTF8.GetBytes(src.Password)))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ReverseMap()
                .ForMember(dest => dest.Password,
                    opt => opt.Ignore()); // We never map password back to response

            CreateMap<User, UserResponse>().ReverseMap();

            CreateMap<ShipmentRequest, Shipment>()
    .ForMember(dest => dest.ShipmentId, opt => opt.Ignore()); // Auto handled

            CreateMap<Shipment, ShipmentResponse>()
                .ForMember(dest => dest.DestinationAddress,
                    opt => opt.MapFrom(src => src.Destination.Address))
                .ForMember(dest => dest.VehicleNumber,
                    opt => opt.MapFrom(src => src.Vehicle.VehicleNumber));

            CreateMap<Inventory, InventoryResponse>()
            .ForMember(d => d.WarehouseName,
                o => o.MapFrom(s => s.Warehouse.Name))
            .ForMember(d => d.ProductName,
                o => o.MapFrom(s => s.Product.Name));

        }
    }
}