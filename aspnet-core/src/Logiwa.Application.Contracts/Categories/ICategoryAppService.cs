#region

using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

#endregion

namespace Logiwa.Categories
{
    public interface ICategoryAppService : IApplicationService
    {
        Task<PagedResultDto<CategoryWithNavigationPropertiesDto>> GetListAsync(GetCategoriesInput input);
        Task<CategoryWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);
        Task<CategoryDto> GetAsync(Guid id);
        Task DeleteAsync(Guid id);
        Task HardDeleteAsync(Guid id);
        Task<CategoryDto> CreateAsync(CategoryCreateDto input);
        Task<CategoryDto> UpdateAsync(Guid id, CategoryUpdateDto input);
    }
}