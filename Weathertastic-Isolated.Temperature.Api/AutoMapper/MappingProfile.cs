using AutoMapper;

namespace Weathertastic_Isolated.Temperature.Api.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Weathertastic_Isolated.Temperature.Infrastructure.Models.OpenWeather.CurrentWeatherData, Weathertastic_Isolated.Temperature.Infrastructure.Models.TemperatureModel>()
                .ForMember(dest => dest.CurrentTemperature, m => m.MapFrom(src => src.Main.Temp))
                .ForMember(dest => dest.FeelsLikeTemperature, m => m.MapFrom(src => src.Main.FeelsLike))
                .ForMember(dest => dest.MaximumTemperature, m => m.MapFrom(src => src.Main.TempMax))
                .ForMember(dest => dest.MinimumTemperature, m => m.MapFrom(src => src.Main.TempMin));
        }
    }
}
