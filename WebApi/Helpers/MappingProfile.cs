using Application.DAL.Models;
using AutoMapper;
using Models.DTOs.Account;
using Models.DTOs.Banner;
using Models.DTOs.Banner.Request;
using Models.DTOs.Blog;
using Models.DTOs.Blog.Request;
using Models.DTOs.BlogGroup;
using Models.DTOs.BlogGroup.Request;
using Models.DTOs.Category;
using Models.DTOs.ContactUs;
using Models.DTOs.ContactUs.Request;
using Models.DTOs.Discount;
using Models.DTOs.GroupBanner;
using Models.DTOs.Order;
using Models.DTOs.Product;
using Models.DTOs.Review;
using Models.DTOs.Supplier;
using Models.DTOs.Tag;
using Models.DTOs.Tag.request;
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



            #region Group banner
            CreateMap<GroupBanner, GroupBannerDto>().ReverseMap();
            CreateMap<Banner, GroupBannerDetailDto.Banner>().ReverseMap();
            CreateMap<GroupBanner, GroupBannerDetailDto>().ReverseMap().ForMember(x=> x.Banners, dt=> dt.MapFrom(y=> y.Banners));

            #endregion

            #region banner mapper

            CreateMap<BannerDto, Banner>().ReverseMap().ForMember(x=>x.GroupBanner, y=>y.MapFrom(z=>z.Group) );
            CreateMap<Banner, BannerRequestDto>().ReverseMap();
            #endregion

            #region Contact us
            CreateMap<ContactUs, ContactUsRequestDto>().ReverseMap();
            CreateMap<ContactUs, ContactUsDto>().ReverseMap();
            #endregion

            #region Blog group 
            CreateMap<BlogGroup, BlogGroupDto>().ReverseMap();
            CreateMap<BlogGroup, BlogGroupRequestDto>().ReverseMap();
            CreateMap<BlogGroup, BlogGroupDetailDto>()
            .ForMember(dest => dest.Blogs, opt => opt.MapFrom(src => src.Blogs));

            CreateMap<Blog, BlogGroupDetailDto.BlogDto>()
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.TagBlogs.Select(tb => tb.Tag.TagTitle)));
            #endregion

            #region Tag

            CreateMap<Tag, TagRequsetDto>().ReverseMap();
            CreateMap<Tag, TagDto>().ReverseMap();
            #endregion
            #region Blog

            CreateMap<Blog, BlogRequestDto>().ReverseMap();
            CreateMap<Blog, BlogDto>()
           .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.UserName))
           .ForMember(dest => dest.BlogGroupName, opt => opt.MapFrom(src => src.BlogGroup.Name))
           .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.TagBlogs.Select(tb => tb.Tag.TagTitle).ToList()));

            #endregion
        }
    }
}
