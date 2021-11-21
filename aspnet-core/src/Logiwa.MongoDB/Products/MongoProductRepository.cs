#region

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Logiwa.Categories;
using Logiwa.Codes;
using Logiwa.MongoDB;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Volo.Abp.Domain.Entities;
using Volo.Abp.MongoDB;

#endregion

namespace Logiwa.Products
{
    public class MongoProductRepository : MongoDbFilterRepository<LogiwaMongoDbContext, Product, Guid>,
        IProductRepository
    {
        public MongoProductRepository(IMongoDbContextProvider<LogiwaMongoDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
        
        public async Task<ProductWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync(cancellationToken);
            var query = await GetAggregateAsync(cancellationToken);

            var product = await query
                .Match(e => e.Id == id)
                .Include<Product, Category>(dbContext)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
            
            if (product == null)
            {
                throw new EntityNotFoundException(typeof(Product), id);
            }


            return new ProductWithNavigationProperties
            {
                Product = product,
                Category = product.GetIncluded<Category>()
            };
        }
        
        public virtual async Task<List<Product>> GetChildrenAsync(
            Guid? parentId,
            bool includeDetails = false,
            CancellationToken cancellationToken = default)
        {
            return await (await GetMongoQueryableAsync(cancellationToken))
                .Where(product => product.ParentId.Equals(parentId))
                .ToListAsync(GetCancellationToken(cancellationToken));
        }
        
        public virtual async Task<Product> GetDefaultModelAsync(
            CancellationToken cancellationToken = default)
        {
            return await FindAsync(p => p.Code.Equals(new Code(ProductConsts.CodePrefix,new List<int>{0})), cancellationToken: cancellationToken);
        }
        
        public virtual async Task<Product> GetByNameAsync(
            string title,
            Guid? parentId,
            CancellationToken cancellationToken = default)
        {
            var query = await GetAggregateAsync(cancellationToken);
            return await query
                .Match(product =>  product.Title.ToLower() == title.ToLower() && product.ParentId == parentId)
                .FirstOrDefaultAsync(GetCancellationToken(cancellationToken));
        }
        
        public virtual async Task<List<Product>> GetAllChildrenWithParentCodeAsync(
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
        
        public async Task<List<ProductWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string filters = null,
            string sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync(cancellationToken);
            var query = await GetAggregateAsync(cancellationToken);

            query = query
                .Include<Product, Category>(dbContext);
            query = ApplyFilter(query, filters);
            var products = await query
                .Skip(skipCount).Limit(maxResultCount)
                .ToListAsync(GetCancellationToken(cancellationToken));

            var result = new List<ProductWithNavigationProperties>();

            foreach (var product in products)
            {
                result.Add(new ProductWithNavigationProperties
                {
                    Product = product,
                    Category = product.GetIncluded<Category>()
                });
            }

            return result;
        }
        
        public async Task<List<Product>> GetListAsync(
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

            query = query
                .Include<Product, Category>(dbContext);
            query = ApplyFilter(query, filters);
            return (await query.Count().FirstOrDefaultAsync(cancellationToken))?.Count ?? 0;
        }
        
        protected virtual IAggregateFluent<Product> ApplyFilter(
            IAggregateFluent<Product> query,
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
                {"ProductName", typeof(string)},
                {"Title", typeof(string)},
                {"Description", typeof(string)},
                {"StockQuantity", typeof(int)},
                {"Price", typeof(decimal)},
                {"Category._id", typeof(Guid)},
                {"ParentId", typeof(Guid)},
                {"IsLive", typeof(bool)},
                {"Code", typeof(string)}
            };
            return base.ApplyFilter(query,fields,filters);
        }

        
    }
}