namespace Brimborium.OpenApi.Attributes; 
/// <summary>
/// This represents the attribute entity for the example.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class OpenApiExampleAttribute : Attribute {
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenApiExampleAttribute"/> class.
    /// </summary>
    public OpenApiExampleAttribute(Type example) {
        this.Example = example;
    }

    /// <summary>
    /// Gets the type of the example. It SHOULD be inheriting the <see cref="OpenApiExample{T}"/> class.
    /// </summary>
    public virtual Type Example { get; }
}
