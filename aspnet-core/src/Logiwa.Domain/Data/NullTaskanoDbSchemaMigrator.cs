using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Logiwa.Data
{
    /* This is used if database provider does't define
     * ILogiwaDbSchemaMigrator implementation.
     */
    public class NullLogiwaDbSchemaMigrator : ILogiwaDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}