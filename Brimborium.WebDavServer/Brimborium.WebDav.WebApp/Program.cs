#pragma warning disable CA1416 // Validate platform compatibility
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Collections.Generic;

namespace Brimborium.WebDav.WebApp {
    public static class Program {
        public static void Main(string[] args) {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) {
            return CreateHostBuilderFor(args, false);
        }
        internal static IHostBuilder CreateHostBuilderFor(string[] args, bool swagger) {
            return Host.CreateDefaultBuilder(args)
                //.ConfigureServices(cfg => {
                //    cfg.Configure<IISServerOptions>(opt => {
                //    });
                //})
                //.ConfigureServices(s=> { 
                //})
                .ConfigureLogging(cfg => {
                    cfg.AddConsole(opt => {
                    });
                })
                .ConfigureHostConfiguration(cfg=> {
                    var d = new Dictionary<string, string>();
                    d.Add("App:FeatureSwagger", swagger.ToString());
                    cfg.AddInMemoryCollection(d);
                })
                .ConfigureWebHostDefaults(webBuilder => {
                    //webBuilder.UseIIS();
                    //webBuilder.UseIISIntegration();
                    //webBuilder.UseHttpSys(opt => {
                    //    opt.Authentication.Schemes = Microsoft.AspNetCore.Server.HttpSys.AuthenticationSchemes.NTLM;
                    //    opt.Authentication.AllowAnonymous = false;
                    //    opt.MaxRequestBodySize = null;
                    //});
                    //webBuilder.UseKestrel(opt=> {
                    //    opt.Limits.MaxRequestBodySize = null;
                    //});
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
    public static class SwaggerHostFactory {
        public static IHost CreateHost() {
            return Program.CreateHostBuilderFor(new string[0] , true).Build();
        }
    }
}
