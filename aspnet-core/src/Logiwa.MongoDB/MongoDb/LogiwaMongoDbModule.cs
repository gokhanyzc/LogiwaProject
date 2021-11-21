#region

using Logiwa.Categories;
using Logiwa.Products;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using Volo.Abp;
using Volo.Abp.AuditLogging.MongoDB;
using Volo.Abp.BackgroundJobs.MongoDB;
using Volo.Abp.FeatureManagement.MongoDB;
using Volo.Abp.Identity.MongoDB;
using Volo.Abp.IdentityServer.MongoDB;
using Volo.Abp.LanguageManagement.MongoDB;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.MongoDB;
using Volo.Abp.SettingManagement.MongoDB;
using Volo.Abp.TextTemplateManagement.MongoDB;
using Volo.Saas.MongoDB;
using Volo.Abp.BlobStoring.Database.MongoDB;
using Volo.Abp.Uow;
using MongoDb.Bson.NodaTime;

#endregion

namespace Logiwa.MongoDB
{
    [DependsOn(
        typeof(LogiwaDomainModule),
        typeof(AbpPermissionManagementMongoDbModule),
        typeof(AbpSettingManagementMongoDbModule),
        typeof(AbpIdentityProMongoDbModule),
        typeof(AbpIdentityServerMongoDbModule),
        typeof(AbpBackgroundJobsMongoDbModule),
        typeof(AbpAuditLoggingMongoDbModule),
        typeof(AbpFeatureManagementMongoDbModule),
        typeof(LanguageManagementMongoDbModule),
        typeof(SaasMongoDbModule),
        typeof(TextTemplateManagementMongoDbModule),
        typeof(BlobStoringDatabaseMongoDbModule)
    )]
    public class LogiwaMongoDbModule : AbpModule
    {
        
        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            BsonDefaults.GuidRepresentationMode = GuidRepresentationMode.V2;
            BsonDefaults.GuidRepresentation = GuidRepresentation.Standard;
        }
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            NodaTimeSerializers.Register();
            context.Services.AddMongoDbContext<LogiwaMongoDbContext>(options =>
            {
                options.AddDefaultRepositories();
                options.AddRepository<Product, MongoProductRepository>();
                options.AddRepository<Category, MongoCategoryRepository>();
               

            });

            Configure<AbpUnitOfWorkDefaultOptions>(options =>
            {
                options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
            });
        }
    }
}
