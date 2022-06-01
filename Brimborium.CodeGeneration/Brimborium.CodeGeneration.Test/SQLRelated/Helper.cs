using Microsoft.Extensions.Configuration;

namespace Brimborium.CodeGeneration.SQLRelated {
    public static class Helper {
        private static string? _ConnectionString;
        public static string GetConnectionString() {
            if (string.IsNullOrEmpty(_ConnectionString)) {
                var cb = new Microsoft.Extensions.Configuration.ConfigurationBuilder();
                cb.AddUserSecrets(typeof(UtilitySQLInfoTests).Assembly);
                var configuration = cb.Build();
                _ConnectionString = configuration.GetValue<string>("ConnectionString");
            }
            return _ConnectionString;
        }
    }
}
