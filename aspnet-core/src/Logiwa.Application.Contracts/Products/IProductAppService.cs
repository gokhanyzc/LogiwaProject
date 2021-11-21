#region

using System;
using System.Threading.Tasks;
using Logiwa.Shared;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

#endregion

namespace Logiwa.Products
{
    public interface IProductAppService : IApplicationService
    {
        Task<PagedResultDto<ProductWithNavigationPropertiesDto>> GetListAsync(GetProductsInput input);
        Task<ProductWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);
        Task<ProductDto> GetAsync(Guid id);
        Task<PagedResultDto<LookupDto<Guid?>>> GetCategoryLookupAsync(LookupRequestDto input);
        Task DeleteAsync(Guid id);
        Task HardDeleteAsync(Guid id);
        Task<ProductDto> CreateAsync(ProductCreateDto input);
        Task<ProductDto> UpdateAsync(Guid id, ProductUpdateDto input);
        
        
        
    }
}