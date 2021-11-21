#region

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Logiwa.Codes;
using Volo.Abp.Domain.Repositories;

#endregion


namespace Logiwa.Products
{
    public interface IProductRepository : IRepository<Product, Guid>
    {
        Task<ProductWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );
        
        Task<Product> GetDefaultModelAsync(
            CancellationToken cancellationToken = default);
        
        Task<Product> GetByNameAsync(
            string title,
            Guid? parentId,
            CancellationToken cancellationToken = default);
        
        Task<List<Product>> GetChildrenAsync(
            Guid? parentId,
            bool includeDetails = false,
            CancellationToken cancellationToken = default
        );

        Task<List<Product>> GetAllChildrenWithParentCodeAsync(
            Code code,
            Guid? parentId,
            bool includeDetails = false,
            CancellationToken cancellationToken = default
        );


        Task<List<ProductWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filters = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Product>> GetListAsync(
            string filters = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<long> GetCountAsync(
            string filters = null,
            CancellationToken cancellationToken = default);
    }
}