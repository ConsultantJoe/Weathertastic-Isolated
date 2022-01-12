using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Weathertastic_Isolated.Temperature.Infrastructure.Models.OpenWeather
{
    public class Wind
    {
        [JsonProperty("speed")]
        public double Speed { get; set; }

        [JsonProperty("deg")]
        public int Deg { get; set; }
    }
}
