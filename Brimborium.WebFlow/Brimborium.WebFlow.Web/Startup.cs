using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Brimborium.CodeFlow.RequestHandler;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Brimborium.WebFlow.Web {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            services.AddRequestHandler();
            services.AddWebFlowServices();
            services.AddServicesWithRegistrator((a) => {
                var assemblies = a.FromAssemblyOf<Startup>();
                CodeFlowExtensions.AddRequestHandlerServices(assemblies);
                assemblies.AddClasses().UsingAttributes();
            });
            services.AddRazorPages();
            services.AddControllers();
            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Brimborium.WebFlow.WebApp", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Brimborium.CodeFlow.WebApp v1"));

            // app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
