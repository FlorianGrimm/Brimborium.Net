namespace Brimborium.TestSampleEfCore;
public class Program {
    public static void Main(string[] args) {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => {
                webBuilder.UseKestrel();
                webBuilder.UseStartup<Startup>();
            });
}


/*
 
set codegen_trace=1

Scaffold-DbContext "host=server;database=test;user id=postgres;" Devart.Data.PostgreSql.Entity.EFCore
Scaffold-DbContext "Data Source=parado.dev.solvin.local;Initial Catalog=TodoDB;Integrated Security=true;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -Namespace Brimborium.TestSampleEfCore.Record -NoPluralize -OutputDir Record -DataAnnotations
Scaffold-DbContext "Data Source=parado.dev.solvin.local;Initial Catalog=TodoDB;Integrated Security=true;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer -Namespace Brimborium.TestSampleEfCore.Record -NoPluralize -OutputDir Record -force

If you have other tables in your database, you may use additional parameters - -Schemas and -Tables - to filter the list of schemas and/or tables that are added to the model. For example, you can use the following command:

Scaffold-DbContext "host=server;database=test;user id=postgres;" Devart.Data.PostgreSql.Entity.EFCore -Tables dept,emp

dotnet-aspnet-codegenerator controller --project:C:\github.com\FlorianGrimm\Brimborium.Net\Brimborium.TypedSql\Brimborium.TestSampleEfCore\Brimborium.TestSampleEfCore.csproj --model:Project --dataContext:TodoDBContext --controllerName:ProjectsController --useAsyncActions --restWithNoViews 
dotnet-scaffold controller --project C:\github.com\FlorianGrimm\Brimborium.Net\Brimborium.TypedSql\Brimborium.TestSampleEfCore\Brimborium.TestSampleEfCore.csproj --model:Project --dataContext:TodoDBContext --controllerName:ProjectsController --useAsyncActions --restWithNoViews 
dotnet-scaffold controller --model:Project --dataContext:TodoDBContext --controllerName:ProjectsController --useAsyncActions --restWithNoViews 

--no-build
*/