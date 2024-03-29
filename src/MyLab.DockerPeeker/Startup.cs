using System;
using Docker.DotNet;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MyLab.DockerPeeker.Services;
using MyLab.DockerPeeker.Tools;
using MyLab.StatusProvider;
using MyLab.WebErrors;
using Newtonsoft.Json;

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
        public void ConfigureServices(IServiceCollection srv)
        {
            srv.AddLogging(b => b.AddConsole());
            srv.AddControllers(c => c.AddExceptionProcessing())
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                });
            
            srv.AddSingleton<DockerCaller>();
            srv.AddSingleton<IPeekingReportService, PeekingReportService>();
            srv.AddSingleton<ICGroupDetector, CGroupDetector>();
            srv.AddSingleton<IContainerStateProvider, DockerContainerStateProvider>();
            srv.AddSingleton<IContainerMetricsProviderRegistry, ContainerMetricsProviderRegistry>();
            srv.AddSingleton<MetricsReportBuilder>();
            srv.AddAppStatusProviding();

            srv.AddSingleton<IFileContentProviderV1, FileContentProviderV1>();
            srv.AddSingleton<IFileContentProviderV2, FileContentProviderV2>();

            srv.Configure<DockerPeekerOptions>(Configuration.GetSection("DockerPeeker"));
            srv.Configure<ExceptionProcessingOptions>(o => o.HideError = false);
#if DEBUG
            srv.Configure<ExceptionProcessingOptions>(o => o.HideError = false);
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
                
            app.UseStatusApi();
        }
    }
}
