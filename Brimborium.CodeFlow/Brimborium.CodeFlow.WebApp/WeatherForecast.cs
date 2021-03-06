using System;

namespace Brimborium.CodeFlow.WebApp {
    public class WeatherForecast {
        public WeatherForecast() {
            this.Summary = string.Empty;
        }

        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}
