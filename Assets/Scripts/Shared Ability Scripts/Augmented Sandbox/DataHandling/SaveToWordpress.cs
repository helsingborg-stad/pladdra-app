using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SaveToWordpress : MonoBehaviour
{
    private readonly string wordpressApiUrl = "https://your-wordpress-site.com/wp-json/custom/v1/save-string/";

    public async Task SaveStringToWordPress(string data, Dictionary<string, string> headers = null)
    {
        Debug.Log("Saving string to WordPress");

        using var request = new HttpRequestMessage(HttpMethod.Post, wordpressApiUrl);
        request.Content = new StringContent(data, Encoding.UTF8, "application/json");

        if (headers != null)
        {
            foreach (var header in headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        using var httpClient = new HttpClient();
        using var response = await httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}
