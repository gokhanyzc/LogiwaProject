using System.Threading.Tasks;

namespace Logiwa.Data
{
    public interface ILogiwaDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}