namespace Brimborium.OpenApi.Attributes; 

/// <summary>
/// This represents the attribute entity for <see cref="IOpenApiCustomUIOptions"/> to be excluded from auto-loading.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class OpenApiCustomUIOptionsIgnoreAttribute : Attribute {
}
