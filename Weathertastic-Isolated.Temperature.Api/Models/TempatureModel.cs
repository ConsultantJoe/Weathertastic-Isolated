using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Weathertastic_Isolated.Temperature.Api.Models
{
    public class TempatureModel
    {
        public int CurrentTempature { get; set; }
        public int FeelsLikeTempature { get; set; }
        public int MinimumTempature { get; set; }
        public int MaximumTempature { get; set; }
    }
}
