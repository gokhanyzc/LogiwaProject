using Logiwa.MongoDB;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Modularity;

namespace Logiwa.DbMigrator
{
    [DependsOn(
        typeof(AbpAutofacModule),
        typeof(LogiwaMongoDbModule),
        typeof(LogiwaApplicationContractsModule)
    )]
    public class LogiwaDbMigratorModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpBackgroundJobOptions>(options =>
            {
                options.IsJobExecutionEnabled = false;
            });
        }
    }
}