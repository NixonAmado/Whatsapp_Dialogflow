using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace WebHook8.Services
{
    public class WhatsappCloudSendMessage : IWhatsappCloudSendMessage
    {
        public async Task<bool> Execute(object model)
        {
            HttpClient client = new HttpClient();
            var byteData = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model));

            using (var content = new ByteArrayContent(byteData))
            {
                string endpoint = "https://graph.facebook.com";
                string phoneNumberId = "382208441653581";
                string accessToken = "EAAPqsJ21vWUBO6U8MZBkjGy5l19K5iJmZBTHtbUizZBbtPLUvqPuScZCfVGGVwbCsqBKJdYe4ZBZCSKfnUTjUsveMhZCVCAXHIRZCFUN7NP0zGu5illHmCwg58OX8PMZAiXgkp6w66UzdYZB3UTBYzDY0OdpC4TiznU7K86P5cDaGxWFMRZCYWkgVCYyNotWj9f1ZBWQ";
                string uri = $"{endpoint}/v20.0/{phoneNumberId}/messages";
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                
                var response = await client.PostAsync(uri,content);

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
