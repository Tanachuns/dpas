
using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

class LineMessageService
{
    public class LineMessageConfig
    {
        public required string BaseUrl { get; set; }

        public required string ClientId { get; set; }
        public required string ClientSecret { get; set; }

    }

    public class LineMessageRequest(string msg)
    {
        public string Type { get; set; } = "text";
        public string Text { get; set; } = msg;
    }

    public async Task<bool> Broadcast(LineMessageConfig config, AlertEntity[] alerts)
    {
        using (HttpClient client = new HttpClient())
        {
            client.BaseAddress = new Uri(config.BaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            string token = await GetTokenAsync(config);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            List<LineMessageRequest> lineMessages = [];
            foreach (var alert in alerts)
            {
                string msg = $"{alert.RegionId.RegionId} has {alert.DisasterType} Risk!";
                lineMessages.Add(new LineMessageRequest(msg));
            }
            HttpContent jsonContent = JsonContent.Create(new
            {
                Messages = lineMessages
            });
            var response = await client.PostAsync("/v2/bot/message/broadcast", jsonContent);

            return response.IsSuccessStatusCode;
        }
    }

    private async Task<string> GetTokenAsync(LineMessageConfig config)
    {
        string token = "";
        using (var client = new HttpClient())
        {

            var formData = new FormUrlEncodedContent([
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id",config.ClientId),
                new KeyValuePair<string, string>("client_secret", config.ClientSecret)]
            );

            client.BaseAddress = new Uri(config.BaseUrl);
            var response = await client.PostAsync("/v2/oauth/accessToken", formData);
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var responseData = JObject.Parse(responseBody);

                token = (string)responseData.SelectToken("access_token");
            }
            return token;
        }
    }
}