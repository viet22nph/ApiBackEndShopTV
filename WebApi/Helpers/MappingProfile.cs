using Application.DAL.Models;
using AutoMapper;
using Models.DTOs.Account;
using Models.DTOs.Category;
using Models.DTOs.Discount;
using Models.DTOs.Order;
using Models.DTOs.Product;
using Models.DTOs.Review;
using Models.DTOs.Supplier;
using Models.DTOs.User;
using Models.Models;

namespace WebApi.Helpers
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();

            CreateMap<Category, CategoryRequest>().ReverseMap();
            CreateMap<Supplier, SupplierDto>().ReverseMap();

            CreateMap<Supplier, SupplierRequest>().ReverseMap();

            CreateMap<Product, ProductDto>()
                .ForMember(p=>p.CategoryName, c=> c.MapFrom(s=>s.Category.Name))
                .ForMember(p=> p.SupplierName, c=> c.MapFrom(s=>s!.Supplier.SupplierName))
                .ReverseMap();
            CreateMap<Product, ProductResponse>()
            .ForMember(p => p.Category, c => c.MapFrom(s => s.Category))
            .ForMember(p=> p.Supplier, c=> c.MapFrom(s=> s.Supplier))
            .ReverseMap();

            CreateMap<Category, ProductResponse.CategoryProduct>().ReverseMap();
            CreateMap<Supplier, ProductResponse.SupplierProduct>().ReverseMap();

            CreateMap<ProductImage, ProductImageDto>().ReverseMap();
            CreateMap<ProductItem, ProductItemDto>().ReverseMap();
            CreateMap<ProductSpecification, ProductSpecificationDto>().ReverseMap();
            CreateMap<Color, ColorDto>().ReverseMap();
            CreateMap<Color, ColorResponse>().ReverseMap();
            CreateMap<ProductItemResponse, ProductItem>().ReverseMap();
            CreateMap<ProductRequest, Product>().ReverseMap();
            CreateMap<ProductItemRequest, ProductItem>().ReverseMap();
            CreateMap<ProductImageRequest, ProductImage>().ReverseMap();
            CreateMap<ProductSpecificationRequest, ProductSpecification>().ReverseMap();
            CreateMap<Discount, DiscountDto>()
                .ForMember(d=>d.Condition, dt=> dt.MapFrom(s=> s.MinimumPurchase))
                .ReverseMap();
            CreateMap<Discount, DiscountRequest>().ForMember(d => d.Condition, dt => dt.MapFrom(s => s.MinimumPurchase))
                .ReverseMap();

            CreateMap<OrderDto, Order>().ReverseMap();

            CreateMap<TransactionDto, Transaction>().ReverseMap();
            
            CreateMap<UserDto, ApplicationUser>().ReverseMap();

            CreateMap<Review, ReviewRequest>().ReverseMap();
            CreateMap<ReviewImage, ReviewImageRequest>().ReverseMap();
            CreateMap<Review, ReviewResponse>().ReverseMap();
            CreateMap<Review, ReviewDto>().ReverseMap();
            CreateMap<ReviewImageResponse, ReviewImage>().ReverseMap();


        }
    }
}
