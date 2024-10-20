using System.Net;
using Newtonsoft.Json.Linq;
using Serilog;

class WeatherApiService
{
    public WeatherApiService(string weatherBaseUrl, string weatherApiKey, string uSGSBaseUrl)
    {
        WeatherBaseUrl = weatherBaseUrl;
        WeatherApiKey = weatherApiKey;
        USGSBaseUrl = uSGSBaseUrl;
    }

    public int RainFallRate { get; set; }
    public decimal Magnitude { get; set; }
    public decimal Temp { get; set; }
    public decimal Humidity { get; set; }

    public string WeatherBaseUrl { get; set; }
    public string WeatherApiKey { get; set; }
    public string USGSBaseUrl { get; set; }





    public async Task FetchWeatherAsync(decimal lat, decimal lon)
    {
        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri(WeatherBaseUrl);
            HttpResponseMessage response = client.GetAsync($"/data/2.5/weather?lat={lat}&lon={lon}&appid={WeatherApiKey}").Result;
            string responseDataString = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseData = JObject.Parse(responseDataString);
                //this.RainFallRate = (int)responseData["sad"];
                this.Temp = (decimal)responseData.SelectToken("main.temp");
                this.Humidity = (decimal)responseData.SelectToken("main.humidity");
            }
            else
            {
                Log.Debug(responseDataString);
                throw new Exception("Weather Api Fectching Error.");
            }
        }
    }

    public async Task FetchUSGSAsync(decimal lat, decimal lon)
    {
        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri(USGSBaseUrl);
            HttpResponseMessage response = client.GetAsync($"/fdsnws/event/1/query?format=geojson&latitude={lat}&longitude={lon}&maxradiuskm=10").Result;
            string responseDataString = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseData = JObject.Parse(responseDataString);
                //this.RainFallRate = (int)responseData["sad"];
                if ((int)responseData.SelectToken("metadata.count") > 0)
                {
                    this.Magnitude = (decimal)responseData.SelectToken("features[0].properties.mag");
                }
            }
            else
            {
                Log.Debug(responseDataString);
                throw new Exception("Api Fectching Error.");
            }
        }
    }
}