#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Logiwa.Categories;
using Logiwa.Codes;
using Logiwa.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MongoDB;

#endregion

namespace Logiwa.Categories
{
    public class MongoCategoryRepository : MongoDbFilterRepository<LogiwaMongoDbContext, Category, Guid>,
        ICategoryRepository
    {
         public MongoCategoryRepository(IMongoDbContextProvider<LogiwaMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
        
        public async Task<CategoryWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync(cancellationToken);
            var query = await GetAggregateAsync(cancellationToken);

            var category = await query
                .Match(e => e.Id == id)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
            
            if (category == null)
            {
                throw new EntityNotFoundException(typeof(Category), id);
            }


            return new CategoryWithNavigationProperties
            {
                Category = category
            };
        }
        
        public virtual async Task<List<Category>> GetChildrenAsync(
            Guid? parentId,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken))
                .Where(category => category.ParentId.Equals(parentId))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }
        
        public virtual async Task<Category> GetDefaultModelAsync(
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(c => c.Code.Equals(new Code(CategoryConsts.CodePrefix,new List<int>{0})), cancellationToken: cancellationToken);
        }
        
        public virtual async Task<Category> GetByNameAsync(
            string name,
            Guid? parentId,
            Guid? exceptId=null,
            CancellationToken cancellationToken = default)
        {
            var query = await GetAggregateAsync(cancellationToken);
            return await query
                .Match(category => category.Id!=exceptId && category.CategoryName.ToLower() == name.ToLower() && category.ParentId == parentId)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }
        
        public virtual async Task<List<Category>> GetAllChildrenWithParentCodeAsync(
            Code code,
            Guid? parentId,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            var query = await GetAggregateAsync(cancellationToken);
            var filters = $@"{{
            'Code': [{{
                'value': '{code}.',
                'matchMode': 'startsWith',
                'operator': 'and'
            }}]
        }}";
            query = ApplyFilter(query, filters);

            var models = await query
                .ToListAsync(GetCancellationToken(cancellationToken));

            return models;
        }
        
        public async Task<List<CategoryWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filters = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync(cancellationToken);
            var query = await GetAggregateAsync(cancellationToken);
            
            query = ApplyFilter(query, filters);
            var categories = await query
                .Skip(skipCount).Limit(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var result = new List<CategoryWithNavigationProperties>();

            foreach (var category in categories)
            {
                result.Add(new CategoryWithNavigationProperties
                {
                    Category = category
                });
            }

            return result;
        }
        
        public async Task<List<Category>> GetListAsync(
            string filters = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var query = await GetAggregateAsync(cancellationToken);
            query = ApplyFilter(query, filters);
            return await query
                .Skip(skipCount).Limit(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));
        }

        public async Task<long> GetCountAsync(
            string filters = null,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync(cancellationToken);
            var query = await GetAggregateAsync(cancellationToken);
            query = ApplyFilter(query, filters);
            return (await query.Count().FirstOrDefaultAsync(cancellationToken))?.Count ?? 0;
        }
        
        protected virtual IAggregateFluent<Category> ApplyFilter(
            IAggregateFluent<Category> query,
            string filters)

        {
            var fields = new Dictionary<string, Type>
            {
                {"Id", typeof(Guid)},
                {"CreationTime", typeof(DateTime)},
                {"CreatorId", typeof(Guid)},
                {"LastModificationTime", typeof(DateTime)},
                {"LastModifierId", typeof(Guid)},
                {"IsDeleted", typeof(bool)},
                {"DeleterId", typeof(Guid)},
                {"DeletionTime", typeof(DateTime)},
                {"CategoryName", typeof(string)},
                {"ParentId", typeof(Guid)},
                {"MinStockQuantity", typeof(int)},
                {"Code", typeof(string)}
            };
            return base.ApplyFilter(query,fields,filters);
        }

    }
}