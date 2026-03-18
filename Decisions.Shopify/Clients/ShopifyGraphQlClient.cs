using ShopifySharp;
using System.Text.Json.Serialization;

namespace Decisions.Shopify.Clients;

public class ShopifyGraphQlClient
{
    public const string CurrentApiVersion = "2026-01";

    private readonly string storeDomain;
    private readonly string accessToken;

    public ShopifyGraphQlClient(string storeDomain, string accessToken)
    {
        this.storeDomain = storeDomain;
        this.accessToken = accessToken;
    }

    public async Task<T?> ExecuteAsync<T>(string query, Dictionary<string, object>? variables = null)
    {
        GraphService graphService = new(storeDomain, accessToken, CurrentApiVersion);

        GraphRequest request = new()
        {
            query = query,
            variables = variables ?? new Dictionary<string, object>()
        };

        GraphQlResponse<T>? response = await graphService.SendAsync<GraphQlResponse<T>>(request, null, CancellationToken.None);
        if (response is null)
        {
            return default;
        }

        return response.Data;
    }

    private sealed class GraphQlResponse<TData>
    {
        [JsonPropertyName("data")]
        public TData? Data { get; set; }
    }
}
