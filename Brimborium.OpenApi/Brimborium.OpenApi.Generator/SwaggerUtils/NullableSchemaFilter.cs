namespace DataverseServiceBusWebApp.SwaggerUtils;

public class NullableSchemaFilter : ISchemaFilter {
    private readonly NullabilityInfoContext _NullabilityInfoContext;

    public NullableSchemaFilter() {
        this._NullabilityInfoContext = new NullabilityInfoContext();
    }

    public void Apply(OpenApiSchema schema, SchemaFilterContext context) {
        if (context.MemberInfo is null && context.ParameterInfo is null) {

            var typeProperties = context.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (var typeProperty in typeProperties) {
                var namePropertyCamelCase = System.Text.Json.JsonNamingPolicy.CamelCase.ConvertName(typeProperty.Name);

                if (schema.Properties.TryGetValue(namePropertyCamelCase, out var schemaProperty)) {
                    applyPropertyInfo(schemaProperty, typeProperty);
                }
            }
        } else if (context.MemberInfo is PropertyInfo propertyInfo) {
            applyPropertyInfo(schema, propertyInfo);
        }
        /*
        else if (context.MemberInfo is not null) {
        } else if (context.ParameterInfo is not null) {
        } else if (context.Type is not null) {
        }
        */
    }

    private void applyPropertyInfo(OpenApiSchema schema, PropertyInfo propertyInfo) {
        if (schema.Nullable) {
            if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) is not null) {

            } else {
                var nullabilityInfo = this._NullabilityInfoContext.Create(propertyInfo);
                if (nullabilityInfo.ReadState == NullabilityState.NotNull) {
                    //var schemaProperty = schema.Properties[context.MemberInfo.Name];
                    //if (schemaProperty is not null) {
                    //    schemaProperty.Nullable = false;
                    //}
                    schema.Nullable = false;
                } else if (nullabilityInfo.ReadState == NullabilityState.Nullable) {
                    schema.Nullable = true;
                }
            }
        } else {
            if (Nullable.GetUnderlyingType(propertyInfo.PropertyType) is not null) {
                schema.Nullable = true;
            } else {
                var nullabilityInfo = this._NullabilityInfoContext.Create(propertyInfo);
                if (nullabilityInfo.ReadState == NullabilityState.Nullable) {
                    schema.Nullable = true;
                }
            }
        }

        if (schema.Reference != null && schema.Nullable) {

            var reference = schema.Reference;
            schema.Reference = null;
            schema.AnyOf.Add(new OpenApiSchema() {
                Type = "null"
            });
            schema.AnyOf.Add(new OpenApiSchema() {
                Reference = reference
            });
        }
    }
}
