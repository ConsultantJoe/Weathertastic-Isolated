using System;
using System.Collections.Generic;
using System.Text;

namespace Weathertastic_Isolated.Temperature.Infrastructure.Models
{
    public class TemperatureModel
    {
        public double CurrentTemperature { get; set; }
        public double FeelsLikeTemperature { get; set; }
        public double MinimumTemperature { get; set; }
        public double MaximumTemperature { get; set; }
    }
}
