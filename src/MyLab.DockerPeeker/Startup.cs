using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyLab.DockerPeeker.Services;
using MyLab.DockerPeeker.Tools;
using MyLab.WebErrors;

namespace MyLab.DockerPeeker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(b => b.AddConsole());
            services.AddControllers(c => c.AddExceptionProcessing());
            services.AddSingleton<ContainersLabelsSource>();
            services.AddSingleton<IContainerLabelsProvider, DockerContainerLabelsProvider>();
            services.AddSingleton<IDockerStatProvider, DockerStatProvider>();
            services.AddSingleton<IContainerMetricsProviderRegistry, ContainerMetricsProviderRegistry>();
            services.AddSingleton<IContainerListProvider, ContainerListProvider>();
            services.AddSingleton<MetricsReportBuilder>();
            services.AddSingleton<IFileContentProvider, RealFileContentProvider>();

#if DEBUG
            services.Configure<ExceptionProcessingOptions>(o => o.HideError = false);
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
