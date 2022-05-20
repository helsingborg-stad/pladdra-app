using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Repository.WP.Schema;

namespace Data
{
    public class WebRestManager
    {
        public async Task<T> GetJson<T>(string endpoint)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

            using var response = await new HttpClient().SendAsync(request);
            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(body);
        }

        public async Task PutJson<T>(string endpoint, T payload, Action<HttpRequestMessage> configure)
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8,
                "application/json");
            configure(request);

            using var response = await new HttpClient().SendAsync(request);
            response.EnsureSuccessStatusCode();
        }
    }
}