using AutoMapper;
using Moq;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using Weathertastic_Isolated.Temperature.Infrastructure.Models.OpenWeather;
using Weathertastic_Isolated.Temperature.Infrastructure.Services;
using Xunit;

namespace Weathertastic_Isolated.Temperature.Infrastructure.Tests
{
    public class WeatherServiceTests
    {
        private readonly Mock<ILogger> _loggerMock;
        private readonly IMapper _mapper;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private MockHttpMessageHandler _httpClientMock;
        public WeatherServiceTests()
        {
            _loggerMock = new Mock<ILogger>();
            _loggerMock.Setup(s => s.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>()));
            _mapper = GetConfiguredMapper();
            _httpClientMock = new MockHttpMessageHandler();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpClientFactoryMock.Setup(s => s.CreateClient(It.IsAny<string>())).Returns(_httpClientMock.ToHttpClient());
        }
        [Fact]
        public void ConstructorThrowsExceptionWhenMissingHttpClient()
        {
            Assert.Throws<ArgumentNullException>(
                () => new WeatherService(null, new WeatherServiceSettings(), _mapper, _loggerMock.Object)
            );
        }

        [Fact]
        public void ConstructorThrowsExceptionWhenMissingWeatherServiceSettings()
        {
            Assert.Throws<ArgumentNullException>(
                    () => new WeatherService(_httpClientFactoryMock.Object, null, _mapper, _loggerMock.Object)
                );
        }

        [Fact]
        public void ConstructorThrowsExceptopnWhenMissingIMapper()
        {
            Assert.Throws<ArgumentNullException>(
                    () => new WeatherService(_httpClientFactoryMock.Object, new WeatherServiceSettings(), null, _loggerMock.Object)
                );
        }
        [Fact]
        public void ConstructorThrowsExceptionWhenMissingILogger()
        {
            Assert.Throws<ArgumentNullException>(
                    () => new WeatherService(_httpClientFactoryMock.Object, new WeatherServiceSettings(), _mapper, null)
                );
        }

        [Fact]
        public void SuccessfulConstructor()
        {
            var weatherServiceSettings = GetSettings();
            var weatherService = new WeatherService(_httpClientFactoryMock.Object, weatherServiceSettings, _mapper, _loggerMock.Object);
            Assert.NotNull(weatherService);
        }

        [Fact]
        public void GetTempatureByCityReturnsData()
        {
            var weatherServiceSettings = GetSettings();
            var openWeatherData = File.ReadAllText("Data/WeatherData.json");
            var currentWeatherData = JsonConvert.DeserializeObject<CurrentWeatherData>(openWeatherData);
            _httpClientMock.When($"{weatherServiceSettings.WeatherServiceBaseUri}")
                .Respond("application/json", openWeatherData);
            
           
            var weatherService = new WeatherService(_httpClientFactoryMock.Object, weatherServiceSettings, _mapper, _loggerMock.Object);
            var weatherData = weatherService.GetTempatureByCity("Denver").Result;

            Assert.NotNull(weatherData);
            _loggerMock.Verify(v => v.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>()), Times.AtLeastOnce);
            Assert.Equal(currentWeatherData.Main.Temp, weatherData.CurrentTemperature);
            Assert.Equal(currentWeatherData.Main.FeelsLike, weatherData.FeelsLikeTemperature);
            Assert.Equal(currentWeatherData.Main.TempMax, weatherData.MaximumTemperature);
            Assert.Equal(currentWeatherData.Main.TempMin, weatherData.MinimumTemperature);
        }

        [Fact]
        public void GetTempatureThrowsException()
        {
            var weatherServiceSettings = GetSettings();
            var httpMock = new MockHttpMessageHandler();
            _httpClientMock.When($"{weatherServiceSettings.WeatherServiceBaseUri}")
                .Respond(HttpStatusCode.InternalServerError);

            var weatherService = new WeatherService(_httpClientFactoryMock.Object, weatherServiceSettings, _mapper, _loggerMock.Object);
            Assert.ThrowsAsync<HttpRequestException>(
                    () => weatherService.GetTempatureByCity("Denver")
                );

        }
        
        private WeatherServiceSettings GetSettings()
        {
            return new WeatherServiceSettings
            {
                WeatherServiceBaseUri = "https://api.openweathermap.org/data/2.5/weather",
                WeatherServiceApiKey = "TestAPIKey",
                WeatherServiceMaxRetryAttempts = 1,
                WeatherServiceLengthBetweenRetries = new TimeSpan(0, 0, 1)
            };
        }
        
        private Mapper GetConfiguredMapper()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
           {
               cfg.CreateMap<Weathertastic_Isolated.Temperature.Infrastructure.Models.OpenWeather.CurrentWeatherData, Weathertastic_Isolated.Temperature.Infrastructure.Models.TemperatureModel>()
                    .ForMember(dest => dest.CurrentTemperature, m => m.MapFrom(src => src.Main.Temp))
                    .ForMember(dest => dest.FeelsLikeTemperature, m => m.MapFrom(src => src.Main.FeelsLike))
                    .ForMember(dest => dest.MaximumTemperature, m => m.MapFrom(src => src.Main.TempMax))
                    .ForMember(dest => dest.MinimumTemperature, m => m.MapFrom(src => src.Main.TempMin));

           });
            
           return new Mapper(mapperConfig);
        }

    }
}
