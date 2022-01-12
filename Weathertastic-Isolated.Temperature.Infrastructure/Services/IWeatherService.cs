using System.Threading.Tasks;
using Weathertastic_Isolated.Temperature.Infrastructure.Models;

namespace Weathertastic_Isolated.Temperature.Infrastructure.Services
{
    public interface IWeatherService
    {
        Task<TemperatureModel> GetTempatureByCity(string city);
    }
}
