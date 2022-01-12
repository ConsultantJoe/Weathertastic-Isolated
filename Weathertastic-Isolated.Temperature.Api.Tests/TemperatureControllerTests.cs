using Microsoft.AspNetCore.Mvc;
using Moq;
using Serilog;
using Serilog.Events;
using System;
using Weathertastic_Isolated.Temperature.Api.Controllers;
using Weathertastic_Isolated.Temperature.Infrastructure.Models;
using Weathertastic_Isolated.Temperature.Infrastructure.Services;
using Xunit;

namespace Weathertastic_Isolated.Temperature.Api.Tests
{
    public class TemperatureControllerTests
    {
        private Mock<ILogger> _loggerMock;
        private Mock<IWeatherService> _weatherServiceMock;
        public TemperatureControllerTests()
        {
            _loggerMock = new Mock<ILogger>();
            _loggerMock.Setup(s => s.Write(It.IsAny<LogEventLevel>(), It.IsAny<string>()));
            _weatherServiceMock = new Mock<IWeatherService>();
        }

        [Fact]
        public void ConstructorThrowsExceptionWhenMissingILogger()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new TemperatureController(null, _weatherServiceMock.Object);
            });
        }

        [Fact]
        public void ConstructorThrowsExceptionWhenMissingIWeatherService()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new TemperatureController(_loggerMock.Object, null);
            });
        }

        [Fact]
        public void SuccessfulConstructor()
        {
            var temperatureController = new TemperatureController(_loggerMock.Object, _weatherServiceMock.Object);
            Assert.NotNull(temperatureController);
        }

        [Fact]
        public void GetTemperatureByCityReturnsData()
        {
            var temperatureModel = new TemperatureModel
                {
                    CurrentTemperature = 73.5,
                    FeelsLikeTemperature = 75.1,
                    MaximumTemperature = 78.9,
                    MinimumTemperature = 62.3
                };

            _weatherServiceMock.Setup(s => s.GetTempatureByCity(It.IsAny<string>()))
                .ReturnsAsync(temperatureModel);
            
            //Setup logger mock 
            var temperatureController = new TemperatureController(_loggerMock.Object, _weatherServiceMock.Object);
            var result = temperatureController.GetTemperatureByCity("Denver").Result;
            Assert.IsType<OkObjectResult>(result);
            var response = result as OkObjectResult;
            Assert.NotNull(response);
            var temperatureData = response.Value as TemperatureModel;
            Assert.NotNull(temperatureData);
            Assert.Equal(temperatureModel.CurrentTemperature, temperatureData.CurrentTemperature);
            Assert.Equal(temperatureModel.FeelsLikeTemperature, temperatureData.FeelsLikeTemperature);
            Assert.Equal(temperatureModel.MaximumTemperature, temperatureData.MaximumTemperature);
            Assert.Equal(temperatureModel.MinimumTemperature, temperatureData.MinimumTemperature);
        }
    }
}
