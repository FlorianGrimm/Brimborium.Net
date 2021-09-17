using Brimborium.CodeFlow.RequestHandler;
using Brimborium.CodeFlow.WebApp.Commands;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.WebApp.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase {
        private readonly IScopeRequestHandlerFactory _ScopeRequestHandlerFactory;

        public WeatherForecastController(IScopeRequestHandlerFactory scopeRequestHandlerFactory) {
            this._ScopeRequestHandlerFactory = scopeRequestHandlerFactory;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get() {
            var ctxt = new RequestHandlerRootContext(this.HttpContext.RequestServices);
            var weatherForecastRequestHandler = this._ScopeRequestHandlerFactory.CreateRequestHandler<IWeatherForecastRequestHandler>();
            var result = await weatherForecastRequestHandler.ExecuteAsync(new WeatherForecastRequest(), ctxt, this.HttpContext.RequestAborted);
            return result.Items;
        }
    }
}
