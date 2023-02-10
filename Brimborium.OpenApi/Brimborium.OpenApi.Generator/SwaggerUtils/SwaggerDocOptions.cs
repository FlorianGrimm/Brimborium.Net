namespace DataverseServiceBusWebApp.SwaggerUtils {
    /*
dotnet run -- --Swagger:Generate=true --Swagger:OutputPath=C:\github.com\FlorianGrimm\TestOrleans2\src\SEW.TestPlanning.Client\openapi.json
<Exec Command="dotnet run -- --Swagger:Generate=true --Swagger:OutputPath=C:\github.com\FlorianGrimm\TestOrleans2\src\SEW.TestPlanning.Client\openapi.json" />
dotnet msbuild /t:SwaggerGenerate

        Swagger:Generate
        Swagger:DocumentName
        Swagger:OutputPath
        Swagger:Host
        Swagger:Basepath
    */
    public class SwaggerDocOptions {
        public SwaggerDocOptions() {
            this.DocumentName = string.Empty;
            this.OutputPath = string.Empty;
            this.Host = string.Empty;
            this.Basepath = string.Empty;
            this.OpenApiInfo = new OpenApiInfo();
        }

        public bool Generate { get; set; }
        public string DocumentName { get; set; }
        public string OutputPath { get; set; }
        public bool Yaml { get; set; }
        public bool SerializeasV2 { get; set; }
        public string Host { get; set; }
        public string Basepath { get; set; }
        public OpenApiInfo OpenApiInfo { get; set; }
    }
}
