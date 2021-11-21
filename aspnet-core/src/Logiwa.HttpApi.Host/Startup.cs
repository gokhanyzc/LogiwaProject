using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Data;
using NodaTime;
using NodaTime.Serialization.SystemTextJson;

namespace Logiwa
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplication<LogiwaHttpApiHostModule>();
            services.AddControllers()
                .AddJsonOptions(delegate(JsonOptions opt)
                {
                    opt.JsonSerializerOptions.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);
                   
                });
            services.Configure<AbpDataFilterOptions>(options =>
            {
                options.DefaultStates[typeof(ISoftDelete)] = new DataFilterState(false);
            });   
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.InitializeApplication();
        }
    }
}
