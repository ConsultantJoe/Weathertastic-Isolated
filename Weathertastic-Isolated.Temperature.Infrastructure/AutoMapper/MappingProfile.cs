using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using Weathertastic_Isolated.Temperature.Infrastructure.Models;
using Weathertastic_Isolated.Temperature.Infrastructure.Models.OpenWeather;

namespace Weathertastic_Isolated.Temperature.Infrastructure.AutoMapper
{
    class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CurrentWeatherData, TemperatureModel>().ForMember(dest => dest.CurrentTemperature, m => m.MapFrom(src => src.Main.Temp)).ReverseMap();
            CreateMap<CurrentWeatherData, TemperatureModel>().ForMember(dest => dest.FeelsLikeTemperature, m => m.MapFrom(src => src.Main.FeelsLike)).ReverseMap();
            CreateMap<CurrentWeatherData, TemperatureModel>().ForMember(dest => dest.MaximumTemperature, m => m.MapFrom(src => src.Main.TempMax)).ReverseMap();
            CreateMap<CurrentWeatherData, TemperatureModel>().ForMember(dest => dest.MinimumTemperature, m => m.MapFrom(src => src.Main.TempMin)).ReverseMap();
        }
    }
}
