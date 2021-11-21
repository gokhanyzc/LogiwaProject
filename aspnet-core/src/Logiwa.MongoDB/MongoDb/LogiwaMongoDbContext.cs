#region

using Logiwa.Categories;
using Logiwa.Products;
using MongoDB.Driver;
using Volo.Abp.Data;
using Volo.Abp.MongoDB;

#endregion

namespace Logiwa.MongoDB
{
    [ConnectionStringName("Default")]
    public class LogiwaMongoDbContext : AbpMongoDbContext
    {

        public IMongoCollection<Product> Products => Collection<Product>();
        public IMongoCollection<Category> Categories => Collection<Category>();
      

        protected override void CreateModel(IMongoModelBuilder modelBuilder)
        {
            base.CreateModel(modelBuilder);

            modelBuilder.Entity<Product>(b => { b.CollectionName = LogiwaConsts.DbTablePrefix + "Products"; });
            modelBuilder.Entity<Category>(b => { b.CollectionName = LogiwaConsts.DbTablePrefix + "Categories"; });
           
            
        }
    }
}
