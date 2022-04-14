using Brimborium.WebSockets;

using Microsoft.AspNetCore.Http;

using System;
using System.Buffers;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection {
    public static class WebSocketExtensions {
        public static IServiceCollection AddWebSocket(this IServiceCollection that) {
            that.AddSingleton<ConnectionManager>();
            return that;
        }
    }
}
namespace Microsoft.AspNetCore.Builder {
    public static class ConnectionManagerExtensions {
        public static IApplicationBuilder UseWebSocket(this IApplicationBuilder app) {
            app.UseMiddleware<WebSocketManagerMiddleware>();
            return app;
        }
    }
}