#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Logiwa.Categories;
using Logiwa.Codes;
using Logiwa.Permissions;
using Logiwa.Shared;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;

#endregion


namespace Logiwa.Products
{
    [RemoteService(IsEnabled = false)]
    public class ProductAppService : LogiwaAppService, IProductAppService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;

        public ProductAppService(IProductRepository productRepository, ICategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }
        
        [Authorize(LogiwaPermissions.Products.Read)]
        public virtual async Task<PagedResultDto<ProductWithNavigationPropertiesDto>> GetListAsync(GetProductsInput input)
        {
            var totalCount = await _productRepository.GetCountAsync(input.Filters);
            var items = await _productRepository.GetListWithNavigationPropertiesAsync(input.Filters, input.Sorting,
                input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<ProductWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<ProductWithNavigationProperties>, List<ProductWithNavigationPropertiesDto>>(items)
            };
        }
        
        [Authorize(LogiwaPermissions.Products.Read)]
        public virtual async Task<ProductWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            return ObjectMapper.Map<ProductWithNavigationProperties, ProductWithNavigationPropertiesDto>
                (await _productRepository.GetWithNavigationPropertiesAsync(id));
        }

        [Authorize(LogiwaPermissions.Products.Read)]
        public virtual async Task<ProductDto> GetAsync(Guid id)
        {
            return ObjectMapper.Map<Product, ProductDto>(await _productRepository.GetAsync(id));
        }
        
        [Authorize]
        public virtual async Task<PagedResultDto<LookupDto<Guid?>>> GetCategoryLookupAsync(LookupRequestDto input)
        {
            var productCategories = _productRepository.AsQueryable().Where(x => x.CategoryId != null)
                .Select(x => x.CategoryId.Value)
                .ToList().Distinct();

            var query = _categoryRepository.AsQueryable().Where(x => productCategories.Contains(x.Id))
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.CategoryName != null && x.CategoryName.Contains(input.Filter));

            var lookupData = await query.OrderBy(ProductConsts.GetDefaultSorting(false))
                .PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Product>();

            var totalCount = query.Count();


            return new PagedResultDto<LookupDto<Guid?>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Product>, List<LookupDto<Guid?>>>(lookupData)
            };
        }
        
        [Authorize(LogiwaPermissions.Products.Delete)]
        public virtual async Task DeleteAsync(Guid id)
        {
            await _productRepository.DeleteAsync(id);
        }

        [Authorize(LogiwaPermissions.Products.HardDelete)]
        public virtual async Task HardDeleteAsync(Guid id)
        {
            await _productRepository.HardDeleteAsync(product => product.Id == id);
        }
        
        [Authorize(LogiwaPermissions.Products.Create)]
        public virtual async Task<ProductDto> CreateAsync(ProductCreateDto input)
        {
            var product = ObjectMapper.Map<ProductCreateDto, Product>(input);
            var category = await _categoryRepository.GetAsync(input.CategoryId.GetValueOrDefault());
            product.Code = await GetNextChildCodeAsync(product.ParentId);
            await ValidateProductAsync(input);
            
           if (product.CategoryId != null)
            {
                if (input.StockQuantity < category.MinStockQuantity)
                {

                    product.IsLive = true;
                    category.MinStockQuantity = category.MinStockQuantity - input.StockQuantity;
                    await _categoryRepository.UpdateAsync(category);

                }
                else
                {
                    product.IsLive = false;
                }
            }
            else
            {
               product.IsLive = false;
            }
           
            product = await _productRepository.InsertAsync(product, true);
            return ObjectMapper.Map<Product, ProductDto>(product);
        }
        
        
        [Authorize(LogiwaPermissions.Products.Edit)]
        public virtual async Task<ProductDto> UpdateAsync(Guid id, ProductUpdateDto input)
        {
          var product = await _productRepository.GetAsync(id);
          var category = await _categoryRepository.GetAsync(input.CategoryId.GetValueOrDefault());
          
          if (product.CategoryId != null)
          {
              if (input.StockQuantity < category.MinStockQuantity)
              {

                  product.StockQuantity = product.StockQuantity + input.StockQuantity;
                  category.MinStockQuantity = category.MinStockQuantity - input.StockQuantity;
                  product.IsLive = true;
                  await _categoryRepository.UpdateAsync(category);
                  await _productRepository.UpdateAsync(product);

              }
              else
              {
                  product.IsLive = false;
                  throw new BusinessException(LogiwaDomainErrorCodes.StockError);
              }
          }
          else
          {
              product.IsLive = false;
          }
          
          return ObjectMapper.Map<Product, ProductDto>(product);
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

            return new Code(ProductConsts.CodePrefix, new List<int> {1});
        }
        
        protected virtual async Task<Product> GetLastChildOrNullAsync(Guid? parentId)
        {
            var children = await _productRepository.GetChildrenAsync(parentId);
            return children.OrderBy(c => c.Code.GetNumber()).LastOrDefault();
        }
        
        
        //VALIDATION
        protected virtual async Task ValidateProductAsync(ProductCreateDto product)
        {
            if (!product.Title.IsNullOrEmpty())
            {
                var duplicate = await _productRepository.GetByNameAsync(product.Title, product.ParentId);

                if (duplicate != null)
                {
                    throw new BusinessException(LogiwaDomainErrorCodes.DuplicateProductName)
                        .WithData("Code", duplicate.Code);
                }
            }
        }

    }
}