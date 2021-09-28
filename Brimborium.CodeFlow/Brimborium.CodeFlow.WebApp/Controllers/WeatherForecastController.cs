using Brimborium.CodeFlow.RequestHandler;
using Brimborium.CodeFlow.WebApp.Commands;

using Microsoft.AspNetCore.Mvc;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.WebApp.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase {
        private readonly IRequestHandlerFactory _RequestHandlerFactory;

        public WeatherForecastController(IRequestHandlerFactory requestHandlerFactory) {
            this._RequestHandlerFactory = requestHandlerFactory;
        }

        [HttpGet]
        public async Task<IEnumerable<WeatherForecast>> Get() {
            var weatherForecastRequestHandler = this._RequestHandlerFactory.CreateRequestHandler<IWeatherForecastRequestHandler>();
            var result = await weatherForecastRequestHandler.ExecuteAsync(new WeatherForecastRequest(), this.HttpContext.RequestAborted);
            return result.Items;
        }
    }
}
