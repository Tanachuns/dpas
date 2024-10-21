using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using StackExchange.Redis;

class WeatherApiService
{
    public WeatherApiService(string weatherBaseUrl, string weatherApiKey, string uSGSBaseUrl, string redisConnectionString)
    {
        WeatherBaseUrl = weatherBaseUrl;
        WeatherApiKey = weatherApiKey;
        USGSBaseUrl = uSGSBaseUrl;
        RedisConnectionString = redisConnectionString;
    }

    public int RainFallRate { get; set; }
    public decimal Magnitude { get; set; }
    public decimal Temp { get; set; }
    public decimal Humidity { get; set; }

    public string WeatherBaseUrl { get; set; }
    public string WeatherApiKey { get; set; }
    public string USGSBaseUrl { get; set; }
    public string RedisConnectionString { get; set; }





    public async Task FetchWeatherAsync(decimal lat, decimal lon)
    {
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(RedisConnectionString ?? throw new Exception("Configuation is null."));
        IDatabase db = redis.GetDatabase();
        string temp = db.StringGet("Temp").ToString();
        string humidity = db.StringGet("Humidity").ToString();

        if (!string.IsNullOrEmpty(temp) && !string.IsNullOrEmpty(humidity))
        {
            this.Temp = decimal.Parse(temp);
            this.Humidity = decimal.Parse(humidity);
            return;
        }
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

                db.StringSet("Temp", this.Temp.ToString(), TimeSpan.FromHours(1));
                db.StringSet("Humidity", this.Humidity.ToString(), TimeSpan.FromHours(1));
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
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(RedisConnectionString ?? throw new Exception("Configuation is null."));
        IDatabase db = redis.GetDatabase();
        string cacheString = db.StringGet("Magnitude").ToString();

        if (!string.IsNullOrEmpty(cacheString))
        {
            this.Magnitude = decimal.Parse(cacheString);
            return;
        }

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
                    db.StringSet("Magnitude", cacheString, TimeSpan.FromHours(1));
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