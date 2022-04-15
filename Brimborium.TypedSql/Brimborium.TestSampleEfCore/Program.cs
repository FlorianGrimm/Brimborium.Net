using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;

using System;
using Microsoft.EntityFrameworkCore;

namespace Brimborium.TestSampleEfCore;

public class Program {
    public static async Task Main(string[] args) {
        try {

            var hostbuilder = Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration((IConfigurationBuilder configurationBuilder) => {
                // configurationBuilder.AddJsonFile();
                    configurationBuilder.AddUserSecrets(typeof(Program).Assembly);
                })
                .ConfigureAppConfiguration((HostBuilderContext hostBuilderContext, IConfigurationBuilder configurationBuilder) => {
                })
                .ConfigureLogging((HostBuilderContext hostBuilderContext, ILoggingBuilder loggingBuilder) => {
                    loggingBuilder.AddConsole();
                })
                .ConfigureServices((HostBuilderContext hostContext, IServiceCollection services) => {
                    var connectionString=hostContext.Configuration.GetConnectionString("Database");
                    services.AddDbContext<Record.TodoDBContext>(optionsAction: (DbContextOptionsBuilder optionsBuilder) => {
                        //optionsBuilder.AddInterceptors();
                    }, 
                    contextLifetime: ServiceLifetime.Scoped,
                    optionsLifetime: ServiceLifetime.Scoped);
                    services.AddHostedService<PingerService>();
                }).
                UseConsoleLifetime()
                ;
            using (var host = hostbuilder.Build()) {
                await host.StartAsync();
            }
        } catch (Exception error) {
            System.Console.Error.WriteLine(error.ToString());
        }
    }

    //public static void ConfigureServices(IServiceCollection services) {

    //    services.AddDbContext<Record.TodoDBContext>(options =>
    //        options.UseSqlServer(Configuration.GetConnectionString("BloggingDatabase")));
    //}
}

internal class PingerService : IHostedService {
    private readonly ILogger<PingerService> _logger;

    public PingerService(ILogger<PingerService> logger) {
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        _logger.LogWarning("Start");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        _logger.LogWarning("Stop");
        return Task.CompletedTask;
    }
}

/*
 
 Scaffold-DbContext "host=server;database=test;user id=postgres;" Devart.Data.PostgreSql.Entity.EFCore
Scaffold-DbContext "Data Source=parado.dev.solvin.local;Initial Catalog=TodoDB;Integrated Security=true;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -Namespace Brimborium.TestSampleEfCore.Record -NoPluralize -OutputDir Record -DataAnnotations

If you have other tables in your database, you may use additional parameters - -Schemas and -Tables - to filter the list of schemas and/or tables that are added to the model. For example, you can use the following command:

Scaffold-DbContext "host=server;database=test;user id=postgres;" Devart.Data.PostgreSql.Entity.EFCore -Tables dept,emp
 */