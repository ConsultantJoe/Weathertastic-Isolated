using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Weathertastic_Isolated.Temperature.Infrastructure.Models.OpenWeather
{
    public class Clouds
    {
        [JsonProperty("all")]
        public int All { get; set; }
    }
}
