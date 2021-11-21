#region

using System;
using System.Threading.Tasks;
using Logiwa.Categories;
using Logiwa.Shared;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

#endregion

namespace Logiwa.Controllers.Categories
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Category")]
    [Route("api/app/categories")]
    public class CategoryController : AbpController, ICategoryAppService
    {
        private readonly ICategoryAppService _categoryAppService;

        public CategoryController(ICategoryAppService categoryAppService)
        {
            _categoryAppService = categoryAppService;
        }
        
        [HttpGet]
        public Task<PagedResultDto<CategoryWithNavigationPropertiesDto>> GetListAsync(GetCategoriesInput input)
        {
            return _categoryAppService.GetListAsync(input);
        }

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<CategoryWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return _categoryAppService.GetWithNavigationPropertiesAsync(id);
        }
        
        [HttpGet]
        [Route("{id}")]
        public virtual Task<CategoryDto> GetAsync(Guid id)
        {
            return _categoryAppService.GetAsync(id);
        }
        
        [HttpPost]
        public virtual Task<CategoryDto> CreateAsync(CategoryCreateDto input)
        {
            return _categoryAppService.CreateAsync(input);
        }

        [HttpPut]
        [Route("{id}")]
        public virtual Task<CategoryDto> UpdateAsync(Guid id, CategoryUpdateDto input)
        {
            return _categoryAppService.UpdateAsync(id, input);
        }

        [HttpDelete]
        [Route("{id}")]
        public virtual Task DeleteAsync(Guid id)
        {
            return _categoryAppService.DeleteAsync(id);
        }
        
        [HttpDelete]
        [Route("{id}/hard")]
        public virtual Task HardDeleteAsync(Guid id)
        {
            return _categoryAppService.HardDeleteAsync(id);
        }
    }
}