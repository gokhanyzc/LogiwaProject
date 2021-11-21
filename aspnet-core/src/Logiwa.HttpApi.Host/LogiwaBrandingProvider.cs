using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Logiwa
{
    [Dependency(ReplaceServices = true)]
    public class LogiwaBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "Logiwa";
    }
}
