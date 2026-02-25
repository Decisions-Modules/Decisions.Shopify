using ShopifySharp;
using ShopifySharp.Services.Graph;

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
            Query = query,
            Variables = variables ?? new Dictionary<string, object>()
        };

        GraphResult<T> response = await graphService.PostAsync<T>(request);
        return response.Data;
    }
}
