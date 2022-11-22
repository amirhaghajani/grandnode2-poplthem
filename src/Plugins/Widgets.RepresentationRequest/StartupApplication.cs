using Grand.Business.Cms.Interfaces;
using Grand.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Widgets.RepresentationRequest.Services;

namespace Widgets.RepresentationRequest
{
    public class StartupApplication: IStartupApplication
    {
        public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IWidgetProvider, RepresentationRequestWidgetProvider>();
            services.AddScoped<IRepresentationRequestService, RepresentationRequestService>();
        }

        public int Priority => 10;
        public void Configure(IApplicationBuilder application, IWebHostEnvironment webHostEnvironment)
        {

        }
        public bool BeforeConfigure => false;
    }
}
