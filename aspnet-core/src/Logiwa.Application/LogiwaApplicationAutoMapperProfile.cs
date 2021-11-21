using System;
using AutoMapper;
using Logiwa.Categories;
using Logiwa.Products;
using Logiwa.Shared;
using Volo.Abp.AutoMapper;

namespace Logiwa
{
    public class LogiwaApplicationAutoMapperProfile : Profile
    {
        public LogiwaApplicationAutoMapperProfile()
        {
            /* You can configure your AutoMapper mapping configuration here.
             * Alternatively, you can split your mapping configurations
             * into multiple profile classes for a better organization. */
            

            CreateMap<Product, LookupDto<Guid?>>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Title));
            CreateMap<ProductWithNavigationProperties, ProductWithNavigationPropertiesDto>();
            CreateMap<ProductCreateDto, Product>().IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id);
            CreateMap<ProductUpdateDto, Product>().IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id);
            CreateMap<Product, ProductDto>();
            
            CreateMap<Category, LookupDto<Guid?>>()
                .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.CategoryName));
            CreateMap<CategoryWithNavigationProperties, CategoryWithNavigationPropertiesDto>();
            CreateMap<CategoryCreateDto, Category>().IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id);
            CreateMap<CategoryUpdateDto, Category>().IgnoreFullAuditedObjectProperties()
                .Ignore(x => x.ExtraProperties).Ignore(x => x.ConcurrencyStamp).Ignore(x => x.Id);
            CreateMap<Category, CategoryDto>();

        }
    }
}