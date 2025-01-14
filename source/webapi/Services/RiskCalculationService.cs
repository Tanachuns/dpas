

public class RiskCalculationService
{
    public static async Task<decimal> CalcurateAsync(RiskCalculationConfig configuration, string disasterType, decimal lat, decimal lon)
    {
        WeatherApiService weatherData = new WeatherApiService(configuration.WeatherBaseUrl, configuration.WeatherApiKey, configuration.USGSBaseUrl, configuration.RedisConnectionString);
        switch (disasterType.ToLower())
        {
            case "earthquake":
                await weatherData.FetchUSGSAsync(lat, lon);
                return weatherData.Magnitude;
            case "flood":
                await weatherData.FetchWeatherAsync(lat, lon);
                return weatherData.RainFallRate;
            case "wildfire":
                await weatherData.FetchWeatherAsync(lat, lon);
                return 100 - weatherData.Humidity + weatherData.Temp;
            default:
                return 0;
        }
    }

    public static string GetLevel(decimal threshold, decimal score)
    {
        if (score < threshold / 3)
        {
            return "Low";
        }
        else if (score < threshold * (2 / 3))
        {
            return "Medium";
        }
        else if (score >= threshold)
        {
            return "High";
        }
        else
        {
            return "Error";
        }
    }

    public class RiskCalculationConfig
    {
        public required string WeatherBaseUrl { get; set; }
        public required string WeatherApiKey { get; set; }
        public required string USGSBaseUrl { get; set; }
        public required string RedisConnectionString { get; set; }
    }
}