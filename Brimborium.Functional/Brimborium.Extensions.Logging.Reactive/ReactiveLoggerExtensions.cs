

namespace Microsoft.Extensions.Logging;
[UnsupportedOSPlatform("browser")]
public static class ReactiveLoggerExtensions {
    public static ILoggingBuilder AddReactiveLogger(
        this ILoggingBuilder builder
        ) {
        builder.AddConfiguration();
        
        builder.Services.TryAdd(ServiceDescriptor.Singleton<IReactiveLoggerSource, ReactiveLoggerSource>());
        builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, ReactiveLoggerProvider>());
        LoggerProviderOptions.RegisterProviderOptions<ReactiveLoggerOptions, ReactiveLoggerProvider>(builder.Services);

        return builder;
    }
    public static ILoggingBuilder AddReactiveLogger(
        this ILoggingBuilder builder, 
        Action<ReactiveLoggerOptions> configure
        ) {
        if (configure == null) {
            throw new ArgumentNullException(nameof(configure));
        }

        builder.AddReactiveLogger();
        builder.Services.Configure(configure);

        return builder;
    }
}
