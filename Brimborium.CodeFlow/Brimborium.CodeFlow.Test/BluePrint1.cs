using System;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.Test {
        public class BluePrint1 {
        public interface IGnaRequestHandler : IRequestHandler<string, string> { }

        public class GnaRequestHandlerFactory : IRequestHandlerFactory<IGnaRequestHandler> {
            public IGnaRequestHandler CreateRequestHandler(IServiceProvider serviceProvider) {
                return new GnaRequestHandler();
            }
        }

        public class GnaRequestHandler : IGnaRequestHandler {
            public async Task<string> ExecuteAsync(string parameters, IContext context) {
                await Task.CompletedTask;
                return parameters + parameters;
            }
        }

        public class GnaRequestHandlerFactoryIntercepted : IRequestHandlerFactory<IGnaRequestHandler> {
            private readonly GnaRequestHandlerFactory _Innerfactory;

            public GnaRequestHandlerFactoryIntercepted(GnaRequestHandlerFactory innerfactory) {
                this._Innerfactory = innerfactory;
            }
            public IGnaRequestHandler CreateRequestHandler(IServiceProvider serviceProvider) {
                return new GnaRequestHandler();
            }
        }

        public class GnaRequestHandlerIntercepted : IGnaRequestHandler {
            private readonly IGnaRequestHandler _InnerHandler;

            public GnaRequestHandlerIntercepted(GnaRequestHandler innerHandler) {
                this._InnerHandler = innerHandler;
            }

            public async Task<string> ExecuteAsync(string parameters, IContext context) {
                var result = await this._InnerHandler.ExecuteAsync(parameters, context);
                return result + "Intercepted";
            }
        }

        [Fact]
        public async Task Test1Async() {
            var s = new ServiceCollection();
            s.AddSingleton<IRequestHandlerFactory<IGnaRequestHandler>, GnaRequestHandlerFactory>();
            s.AddTransient<IContext, Context>();
            s.RemoveAt(0);

            s.AddSingleton<GnaRequestHandlerFactory, GnaRequestHandlerFactory>();
            s.AddSingleton<IRequestHandlerFactory<IGnaRequestHandler>, GnaRequestHandlerFactoryIntercepted>();

            using var globalServiceProvider = s.BuildServiceProvider();
            using var scopre = globalServiceProvider.CreateScope();
            var scopeServiceProvider = scopre.ServiceProvider;

            var act = await controllerMethod("1");
            Assert.Equal("11Intercepted", act);

            async Task<string> controllerMethod(string parameters) {
                var context = scopeServiceProvider.GetRequiredService<Context>();
                var factory = scopeServiceProvider.GetRequiredService<IRequestHandlerFactory<IGnaRequestHandler>>();
                var rh = factory.CreateRequestHandler(scopeServiceProvider);
                var result = await rh.ExecuteAsync(parameters, context);
                return result;

            }
        }
    }
}
