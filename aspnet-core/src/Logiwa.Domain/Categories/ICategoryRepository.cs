#region

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Logiwa.Codes;
using Volo.Abp.Domain.Repositories;

#endregion

namespace Logiwa.Categories
{
    public interface ICategoryRepository : IRepository<Category, Guid>
    {
        Task<CategoryWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default
        );
        
        Task<Category> GetByNameAsync(
            string name,
            Guid? parentId,
            Guid? exceptId=null,
            CancellationToken cancellationToken = default);
        
        Task<List<Category>> GetChildrenAsync(
            Guid? parentId,
            bool includeDetails = false,
            CancellationToken cancellationToken = default
        );

        Task<List<Category>> GetAllChildrenWithParentCodeAsync(
            Code code,
            Guid? parentId,
            bool includeDetails = false,
            CancellationToken cancellationToken = default
        );


        Task<List<CategoryWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filters = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default
        );

        Task<List<Category>> GetListAsync(
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