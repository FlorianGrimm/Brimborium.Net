using System;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.Test {
    public class BluePrint2 {
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

        public class GnaRequestHandlerFactoryChained : IRequestHandlerFactoryChained<IGnaRequestHandler> {
            private readonly Type _ImplementationType;
            private readonly IRequestHandlerFactory<IGnaRequestHandler> _Next;

            public GnaRequestHandlerFactoryChained(Type implementationType, IRequestHandlerFactory<IGnaRequestHandler> next) {
                this._ImplementationType = implementationType;
                this._Next = next;
            }

            public IGnaRequestHandler CreateRequestHandler(IServiceProvider serviceProvider) {
                return new GnaRequestHandlerChained(
                    this._Next.CreateRequestHandler(serviceProvider)
                    );
            }
        }

        public class GnaRequestHandlerChained : IGnaRequestHandler {
            private readonly IGnaRequestHandler _InnerHandler;

            public GnaRequestHandlerChained(IGnaRequestHandler innerHandler) {
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
            s.AddSingleton<IRequestHandlerFactory<IGnaRequestHandler>, GnaRequestHandlerFactoryChained>();

            using var globalServiceProvider = s.BuildServiceProvider();
            using var scopre = globalServiceProvider.CreateScope();
            var scopeServiceProvider = scopre.ServiceProvider;

            var act = await controllerMethod("1");
            Assert.Equal("11Intercepted", act);

            async Task<string> controllerMethod(string parameters) {
                var context = scopeServiceProvider.GetRequiredService<Context>();
                var gnaRequestHandlerFactory = globalServiceProvider.GetRequiredService<GnaRequestHandlerFactory>();
                ObjectFactory objectFactory = ActivatorUtilities.CreateFactory(typeof(GnaRequestHandlerFactoryChained), new Type[] { typeof(Type), typeof(IRequestHandlerFactory<IGnaRequestHandler>) }) ?? throw new InvalidOperationException();
                var gnaRequestHandlerFactoryChained = (GnaRequestHandlerFactoryChained)objectFactory(scopeServiceProvider, new object[] { typeof(GnaRequestHandlerChained), gnaRequestHandlerFactory });




                var factory = scopeServiceProvider.GetRequiredService<IRequestHandlerFactory<IGnaRequestHandler>>();
                var rh = factory.CreateRequestHandler(scopeServiceProvider);
                var result = await rh.ExecuteAsync(parameters, context);
                return result;

            }
        }
    }
}
