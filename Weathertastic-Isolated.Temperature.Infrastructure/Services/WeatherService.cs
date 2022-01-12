using AutoMapper;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using Serilog;
using Serilog.Events;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Weathertastic_Isolated.Temperature.Infrastructure.Models;
using Weathertastic_Isolated.Temperature.Infrastructure.Models.OpenWeather;

namespace Weathertastic_Isolated.Temperature.Infrastructure.Services
{
    /// <summary>
    /// This service pulls in weather data from the OpenWeather service using 
    /// the /weather endpoint
    /// OpenWeather /weather Docs: https://openweathermap.org/current
    /// </summary>
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly WeatherServiceSettings _weatherServiceSettings;
        private readonly ILogger _logger;
        private readonly AsyncPolicy _retryPolicy;
        private readonly IMapper _mapper;
        public WeatherService(IHttpClientFactory httpClientFactory, WeatherServiceSettings weatherServiceSettings, IMapper mapper, ILogger logger)
        {
            var clientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _httpClient = clientFactory.CreateClient();
            _weatherServiceSettings = weatherServiceSettings ?? throw new ArgumentNullException(nameof(weatherServiceSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _retryPolicy = ConfigureRetryPolicy(_weatherServiceSettings.WeatherServiceMaxRetryAttempts, _weatherServiceSettings.WeatherServiceLengthBetweenRetries);
        }

        public async Task<TemperatureModel> GetTempatureByCity(string city)
        {
            var currentWeather = await GetCurrentWeather(city);
            return _mapper.Map<TemperatureModel>(currentWeather);
        }

        private async Task<CurrentWeatherData> GetCurrentWeather(string city)
        {
            var uri = $"{_weatherServiceSettings.WeatherServiceBaseUri}?q={city}&appid={_weatherServiceSettings.WeatherServiceApiKey}&units=imperial";
            var content = string.Empty;
            var logMethod = "WeatherService::GetCurrentWeather - ";
            _logger.Write(LogEventLevel.Information, $"{logMethod}Retrieving Weather Data");
            _logger.Write(LogEventLevel.Debug, $"{logMethod}Weather Service URI: {uri}");

            await _retryPolicy.ExecuteAsync(async () =>
            {

                var response = await _httpClient.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.Write(LogEventLevel.Error, $"{logMethod}Failure when calling OpenWeather Service. Uri: {uri}");
                    response.EnsureSuccessStatusCode();
                }

                content = await response.Content.ReadAsStringAsync();
            });
            
            _logger.Write(LogEventLevel.Information, $"{logMethod}Returning Weather Data");
            _logger.Write(LogEventLevel.Debug, $"{logMethod}Weather Service Response:\n {content}");

            return JsonConvert.DeserializeObject<CurrentWeatherData>(content) ;
        }
            
        private AsyncRetryPolicy ConfigureRetryPolicy(int maxRetryAttempts, TimeSpan lengthBetweenRetries)
        {
            return Policy
                    .Handle<HttpRequestException>()
                    .WaitAndRetryAsync(maxRetryAttempts, t => lengthBetweenRetries);
        }
    }
}
