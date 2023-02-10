namespace DataverseServiceBusWebApp.SwaggerUtils;

public class MimeTypeJsonOperationFilter : IOperationFilter {
    const string mimetype = "application/json";
    public void Apply(OpenApiOperation operation, OperationFilterContext context) {
        if (operation is null) { return; }
        if (operation.RequestBody is not null) {
            var content = operation.RequestBody.Content;
            var keys = content.Keys.ToList();
            if (keys.Count > 1 && keys.Contains(mimetype)) {
                foreach (var key in keys) {
                    if (key == mimetype) {
                    } else {
                        content.Remove(key);
                    }
                }
            }
        }
        if (operation.Responses is not null) {
            foreach (var response in operation.Responses) {
                var content = response.Value.Content;
                var keys = content.Keys.ToList();
                if (keys.Count > 1 && keys.Contains(mimetype)) {
                    foreach (var key in keys) {
                        if (key == mimetype) {
                        } else {
                            content.Remove(key);
                        }
                    }
                }
            }
        }
    }
}
