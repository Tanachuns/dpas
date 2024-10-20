

public class RiskCalculationService
{
    public static async Task<decimal> CalcurateAsync(IConfiguration configuration, string disasterType, decimal lat, decimal lon)
    {
        WeatherApiService weatherData = new WeatherApiService(configuration["Api:Weather:BaseUrl"].ToString(), configuration["Api:Weather:ApiKey"].ToString(), configuration["Api:USGS:BaseUrl"].ToString());
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
}