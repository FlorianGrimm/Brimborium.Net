namespace Brimborium.Configuration;

public static class SqlConfigurationBuilderExtensions {
    public static IConfigurationBuilder AddSqlConfiguration(
            this IConfigurationBuilder builder,
            string connectionStringConfigurationName,
            string connectionString,
            ISqlConfigurationAccess sqlConfigurationAccess
        ) {
        string connectionStringCurrent;
        if (string.IsNullOrEmpty(connectionStringConfigurationName)) {
            // do nothing
            connectionStringCurrent = connectionString;
        } else {
            var tempConfig = builder.Build();
            var connectionStringConfig 
                = tempConfig.GetConnectionString(connectionStringConfigurationName)
                ?? tempConfig.GetSection(connectionStringConfigurationName)?.Value;
            if (string.IsNullOrEmpty(connectionStringConfig)) {
                connectionStringCurrent = connectionString;
            } else {
                connectionStringCurrent = connectionStringConfig;
            }
        }

        if (string.IsNullOrEmpty(connectionStringCurrent)) {
            return builder;
        } else {
            var source = new SqlConfigurationSource(
                connectionStringCurrent,
                sqlConfigurationAccess);
            return builder.Add(source);
        }
    }
}
