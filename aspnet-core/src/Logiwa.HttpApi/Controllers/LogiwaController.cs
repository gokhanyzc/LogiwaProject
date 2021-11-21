using Logiwa.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Logiwa.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class LogiwaController : AbpController
    {
        protected LogiwaController()
        {
            LocalizationResource = typeof(LogiwaResource);
        }
    }
}