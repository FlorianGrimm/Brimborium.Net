using Brimborium.WebDavServer.FileSystem;
using Brimborium.WebDavServer.FileSystem.DotNet;
using Brimborium.WebDavServer.Locking;
using Brimborium.WebDavServer.Locking.InMemory;
using Brimborium.WebDavServer.Props.Store;
using Brimborium.WebDavServer.Props.Store.TextFile;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Brimborium.WebDav.WebApp {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            services.AddControllers();
            services.AddRazorPages();

            services.AddSwaggerGen(c => {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Brimborium.WebDav.WebApp", Version = "v1" });
            });
            services.AddWebDav();
            services.Configure<DotNetFileSystemOptions>(opt=> {
                opt.RootPath = @"C:\temp";
                opt.AnonymousUserName = "";
            });
            services.Configure<TextFilePropertyStoreOptions>(opt => {
                opt.RootFolder = @"C:\temp";
                opt.StoreInTargetFileSystem = true;
            });
            //services.Configure<InMemoryLockManagerOptions>(opt => {
            //});
            services.AddOptions<WebDavServer.AspNetCore.WebDavHostOptions>().Configure(opt => {
                opt.BaseUrl = "http://localhost:4747/webdav";
            });
            
            services.AddScoped<IFileSystemFactory, DotNetFileSystemFactory>();
            services.AddScoped<IPropertyStoreFactory, TextFilePropertyStoreFactory>();
            services.AddSingleton<ILockManager, InMemoryLockManager>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Brimborium.WebDav.WebApp v1"));

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
