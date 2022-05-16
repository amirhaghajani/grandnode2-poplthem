using Grand.Infrastructure;
using Grand.Web.Themes.Popl.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Grand.Web.Themes.Popl
{
    public class Startup : IStartupApplication
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<MyRouteTransformer>();
        }

        public void Configure(IApplicationBuilder application, IWebHostEnvironment webHostEnvironment)
        {
        }


        public int Priority => 0;
        public bool BeforeConfigure => false;

    }
}
