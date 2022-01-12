using System;

namespace Weathertastic_Isolated.Temperature.Infrastructure.Services
{
    public class WeatherServiceSettings
    {
        public string WeatherServiceBaseUri { get; set; }
        public string WeatherServiceApiKey { get; set; }
        public int WeatherServiceMaxRetryAttempts { get; set; }
        public TimeSpan WeatherServiceLengthBetweenRetries { get; set; } 
    }
}
