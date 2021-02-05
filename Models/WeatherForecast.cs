using System;
using System.Collections.Generic;

namespace WeatherApi
{
    public class FiveDayForecast
    {
        public List<OpenWeatherResponse> List { get; set; }
    }
    public class OpenWeatherResponse
    {
        public Main Main { get; set; }
        public Wind Wind { get; set; }
        public Clouds Clouds { get; set; }
        public double Dt { get; set; }
    }

    public class Main
    {
        public string Temp { get; set; }
        public string Temp_min { get; set; }
        public string Temp_max { get; set; }

    }

    public class Wind
    {
        public string Speed { get; set; }
    }
    public class Clouds
    {
        public double All { get; set; }
    }
}
