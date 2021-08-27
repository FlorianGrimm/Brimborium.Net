#pragma warning disable CA1416 // Validate platform compatibility
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;

using Microsoft.AspNetCore.Http;
using System.Runtime.Versioning;

namespace Brimborium.WebDav.AspNetCore.Middlewares {
    [SupportedOSPlatform("windows")]
    public class ImpersonationMiddleware {
        private readonly RequestDelegate _next;

        public ImpersonationMiddleware(RequestDelegate next) {
            _next = next;
        }

        public async Task Invoke(HttpContext context) {
            if (  (context.User.Identity is WindowsIdentity identity)
                && identity.IsAuthenticated
                ) {
                await WindowsIdentity.RunImpersonated(
                    identity.AccessToken,
                    async () => { await _next(context); });
            } else {
                await _next(context);
            }
        }
    }
}
