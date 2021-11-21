#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logiwa.Codes;
using Logiwa.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

#endregion

namespace Logiwa.Categories
{
    [RemoteService(IsEnabled = false)]
    public class CategoryAppService : LogiwaAppService, ICategoryAppService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryAppService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        
        [Authorize(LogiwaPermissions.Categories.Read)]
        public virtual async Task<PagedResultDto<CategoryWithNavigationPropertiesDto>> GetListAsync(GetCategoriesInput input)
        {
            var totalCount = await _categoryRepository.GetCountAsync(input.Filters);
            var items = await _categoryRepository.GetListWithNavigationPropertiesAsync(input.Filters, input.Sorting,
                input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<CategoryWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<CategoryWithNavigationProperties>, List<CategoryWithNavigationPropertiesDto>>(items)
            };
        }
        
        [Authorize(LogiwaPermissions.Categories.Read)]
        public virtual async Task<CategoryWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<CategoryWithNavigationProperties, CategoryWithNavigationPropertiesDto>
                (await _categoryRepository.GetWithNavigationPropertiesAsync(id));
        }

        [Authorize(LogiwaPermissions.Categories.Read)]
        public virtual async Task<CategoryDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Category, CategoryDto>(await _categoryRepository.GetAsync(id));
        }
        
        [Authorize(LogiwaPermissions.Categories.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _categoryRepository.DeleteAsync(id);
        }

        [Authorize(LogiwaPermissions.Categories.HardDelete)]
        public virtual async Task HardDeleteAsync(Guid id)
        {
            await _categoryRepository.HardDeleteAsync(category => category.Id == id);
        }
        
        [Authorize(LogiwaPermissions.Categories.Create)]
        public virtual async Task<CategoryDto> CreateAsync(CategoryCreateDto input)
        {
            var category = ObjectMapper.Map<CategoryCreateDto, Category>(input);
            category.Code = await GetNextChildCodeAsync(category.ParentId);
            category.FirstQuantity = category.MinStockQuantity;
            await ValidateCategoryAsync(input);
            category = await _categoryRepository.InsertAsync(category, true);
            return ObjectMapper.Map<Category, CategoryDto>(category);
        }
        
        [Authorize(LogiwaPermissions.Categories.Edit)]
        public virtual async Task<CategoryDto> UpdateAsync(Guid id, CategoryUpdateDto input)
        {
            var category = await _categoryRepository.GetAsync(id);
            ObjectMapper.Map(input, category); 
            category = await _categoryRepository.UpdateAsync(category);
            return ObjectMapper.Map<Category, CategoryDto>(category);
        }
        
        protected virtual async Task<Code> GetNextChildCodeAsync(Guid? parentId)
        {
            var lastChild = await GetLastChildOrNullAsync(parentId);
            if (lastChild != null)
            {
                return lastChild.Code.Add(1);
            }

            if (parentId != null)
            {
                return new Code((await GetAsync(parentId.Value)).Code).Append(1);
            }

            return new Code(CategoryConsts.CodePrefix, new List<int> {1});
        }
        
        protected virtual async Task<Category> GetLastChildOrNullAsync(Guid? parentId)
        {
            var children = await _categoryRepository.GetChildrenAsync(parentId);
            return children.OrderBy(c => c.Code.GetNumber()).LastOrDefault();
        }
        
        
        //VALIDATION
        protected virtual async Task ValidateCategoryAsync(CategoryCreateDto category)
        {
            if (!category.CategoryName.IsNullOrEmpty())
            {
                var duplicate = await _categoryRepository.GetByNameAsync(category.CategoryName, category.ParentId);

                if (duplicate != null)
                {
                    throw new BusinessException(LogiwaDomainErrorCodes.DuplicateCategoryName)
                        .WithData("Code", duplicate.Code);
                }
            }
        }

        
    }
}
    
