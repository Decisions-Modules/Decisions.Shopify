using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Decisions.Shopify.Clients;

public class ShopifyRestClient
{
    public const string CurrentApiVersion = "2026-01";

    private readonly string storeDomain;
    private readonly string accessToken;
    private static readonly HttpClient HttpClient = new();

    public ShopifyRestClient(string storeDomain, string accessToken)
    {
        this.storeDomain = storeDomain;
        this.accessToken = accessToken;
    }

    public async Task<T?> GetAsync<T>(string relativePath)
    {
        HttpRequestMessage request = new(HttpMethod.Get, BuildUrl(relativePath));
        request.Headers.Add("X-Shopify-Access-Token", accessToken);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        using HttpResponseMessage response = await HttpClient.SendAsync(request);
        string responseBody = await response.Content.ReadAsStringAsync();

        response.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<T>(responseBody);
    }

    private string BuildUrl(string relativePath)
    {
        string trimmedPath = relativePath.TrimStart('/');
        return $"https://{storeDomain}/admin/api/{CurrentApiVersion}/{trimmedPath}";
    }
}
