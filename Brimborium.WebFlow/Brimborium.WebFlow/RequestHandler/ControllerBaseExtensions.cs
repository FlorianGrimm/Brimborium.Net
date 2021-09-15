﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Brimborium.CodeFlow.RequestHandler;

using Microsoft.AspNetCore.Mvc;

// namespace Brimborium.WebFlow.RequestHandler

namespace Brimborium.CodeFlow.RequestHandler {
    public static class ControllerBaseExtensions {
        static ActivitySource s_source = new ActivitySource("Sample.DistributedTracing");

        public static IRequestHandlerContext GetRequestHandlerContext(
            this ControllerBase controllerBase,
            IRequestHandlerContextHolder requestHandlerContextHolder,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = ""
            ) {
            //var requestServices = controllerBase.HttpContext.RequestServices;
            if (requestHandlerContextHolder.TryGetRequestHandlerContext(false, out var result)) {
                return result;
            } else {
                var requestServices = requestHandlerContextHolder.GetScopeServiceProvider();

                //var a = System.Diagnostics.Activity.Current;
#warning TODO https://docs.microsoft.com/en-us/dotnet/core/diagnostics/distributed-tracing-collection-walkthroughs
                
                //var name = $"{controllerBase.GetType().FullName}.{memberName}";
                //Activity? activity = s_source.StartActivity(name, ActivityKind.Server)!;
                //activity.Dispose(); 

                result = new RequestHandlerRootContext(requestServices);
                requestHandlerContextHolder.SetRequestHandlerContext(result);
                return result;
            }
        }
    }
}