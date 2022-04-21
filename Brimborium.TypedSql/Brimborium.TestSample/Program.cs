
using System.Collections.Generic;

namespace Brimborium.TestSample;

public static class Program {
    public static string ConnectionString = "Data Source=.;Initial Catalog=TestDB;Trusted_Connection=True;";

    public static void Main(string[] args) {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => {
                webBuilder.UseStartup<Startup>();
            });

}
