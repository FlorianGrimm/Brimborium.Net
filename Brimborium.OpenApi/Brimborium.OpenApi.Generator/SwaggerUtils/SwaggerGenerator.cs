namespace DataverseServiceBusWebApp.SwaggerUtils;

using Microsoft.OpenApi.Writers;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Builder;

/// <summary>
/// swagger:DocumentName - as defined in code otherwise run webapp.
/// swagger:OutputPath - output filepath otherwise Console.Out.
/// swagger:yaml true:yaml otherwise json.
/// swagger:serializeasv2 - true:v2 otherwise v3.
/// swagger:host - host in swagger.
/// swagger:basepath - basepath in swagger.
/// </summary>
public static class SwaggerGenerator {
    public static bool Generate(
        string[] args,
        WebApplicationBuilder webApplicationBuilder,
        SwaggerDocOptions? swaggerDocOptions = default,
        Action<WebApplicationBuilder>? configureForSwaggerGeneration = default
        ) {

        if (swaggerDocOptions is null) {
            swaggerDocOptions = new SwaggerDocOptions();
        }
        webApplicationBuilder.Configuration.GetSection("Swagger").Bind(swaggerDocOptions);
        
        if (swaggerDocOptions.Generate) {
            if (configureForSwaggerGeneration is not null) {
                configureForSwaggerGeneration(webApplicationBuilder);
            }
            var serviceProvider = webApplicationBuilder.Build().Services;
            var swaggerProvider = serviceProvider.GetRequiredService<ISwaggerProvider>();
            var swagger = swaggerProvider.GetSwagger(
                documentName: swaggerDocOptions.DocumentName,
                host: string.IsNullOrEmpty(swaggerDocOptions.Host) ? null : swaggerDocOptions.Host,
                basePath: string.IsNullOrEmpty(swaggerDocOptions.Basepath) ? null : swaggerDocOptions.Basepath
                );
            var outputPath = string.IsNullOrEmpty(swaggerDocOptions.OutputPath)
                    ? null
                    : Path.Combine(Directory.GetCurrentDirectory(), swaggerDocOptions.OutputPath)
                    ;
            if (string.IsNullOrEmpty(outputPath)) {
                var swSwagger = Console.Out;
                IOpenApiWriter writer = swaggerDocOptions.Yaml
                    ? new OpenApiYamlWriter(swSwagger)
                    : writer = new OpenApiJsonWriter(swSwagger);
                if (swaggerDocOptions.SerializeasV2) {
                    swagger.SerializeAsV2(writer);
                } else {
                    swagger.SerializeAsV3(writer);
                }
            } else {
                var sbSwagger = new StringBuilder();
                using (var swSwagger = new StringWriter(sbSwagger)) {
                    IOpenApiWriter writer = swaggerDocOptions.Yaml
                        ? new OpenApiYamlWriter(swSwagger)
                        : writer = new OpenApiJsonWriter(swSwagger);
                    if (swaggerDocOptions.SerializeasV2) {
                        swagger.SerializeAsV2(writer);
                    } else {
                        swagger.SerializeAsV3(writer);
                    }
                }
                var contentNew = sbSwagger.ToString();
                string contentOld;
                {
                    try {
                        contentOld = File.ReadAllText(outputPath);
                    } catch (FileNotFoundException) {
                        contentOld = string.Empty;
                    }
                }
                if (string.Equals(contentOld, contentNew, StringComparison.Ordinal)) {
                    Console.Out.WriteLine($"Swagger {(swaggerDocOptions.SerializeasV2 ? "V2" : "V3")} {(swaggerDocOptions.Yaml ? "YAML" : "JSON")} is uptodate to {outputPath}.");
                } else {
                    File.WriteAllText(outputPath, contentNew);
                    Console.Out.WriteLine($"Swagger {(swaggerDocOptions.SerializeasV2 ? "V2" : "V3")} {(swaggerDocOptions.Yaml ? "YAML" : "JSON")} successfully written to {outputPath}.");
                }
            }
            return true;
        } else {
            return false;
        }
    }
}
