using Logiwa.Localization;
using Volo.Abp.Application.Services;

namespace Logiwa
{
    /* Inherit your application services from this class.
     */
    public abstract class LogiwaAppService : ApplicationService
    {
        protected LogiwaAppService()
        {
            LocalizationResource = typeof(LogiwaResource);
        }
    }
}
