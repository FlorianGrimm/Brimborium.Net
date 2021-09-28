using Brimborium.CodeFlow.RequestHandler;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.CodeFlow.WebApp.Commands {
    public record WeatherForecastRequest();
    public record WeatherForecastResponse(
        List<WeatherForecast> Items
        );

    public interface IWeatherForecastRequestHandler : IRequestHandler<WeatherForecastRequest, WeatherForecastResponse> { }

    public class WeatherForecastRequestHandler : IWeatherForecastRequestHandler {

        private static readonly string[] _Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public WeatherForecastRequestHandler() {
        }

        public async Task<WeatherForecastResponse> ExecuteAsync(WeatherForecastRequest request, CancellationToken cancellationToken = default) {
            await Task.CompletedTask;
            var rng = new Random();
            var items = Enumerable.Range(1, 5).Select(index => new WeatherForecast {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = _Summaries[rng.Next(_Summaries.Length)]
            })
            .ToList();
            return new WeatherForecastResponse(Items: items);
        }
    }
}
