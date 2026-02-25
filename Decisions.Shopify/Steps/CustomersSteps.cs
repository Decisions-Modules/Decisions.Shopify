using Decisions.Shopify.Clients;
using Decisions.Shopify.Data;
using Decisions.Shopify.Utility;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.Design.Properties.Attributes;

namespace Decisions.Shopify.Steps;

[AutoRegisterMethodsOnClass(true, "Integration/Shopify/Customers")]
public class CustomersSteps
{
    public ShopifyCustomer? GetCustomerById([PropertyClassification(0, "Store Domain", "Connection")] string storeDomain,
      [TokenPicker][PropertyClassification(0, "Token", "Connection")] string tokenId,
      string customerId)
    {
        ShopifyGraphQlClient client = CreateClient(storeDomain, tokenId);

        const string query = @"query ($id: ID!) {
  customer(id: $id) {
    id
    email
    firstName
    lastName
    phone
    tags
  }
}";

        GraphCustomerResult? result = client.ExecuteAsync<GraphCustomerResult>(query, new Dictionary<string, object>
        {
            ["id"] = customerId
        }).GetAwaiter().GetResult();

        return result?.Customer == null ? null : ShopifyMapper.ToCustomer(result.Customer);
    }

    public ShopifyCustomersPage ListCustomers([PropertyClassification(0, "Store Domain", "Connection")] string storeDomain,
      [TokenPicker][PropertyClassification(0, "Token", "Connection")] string tokenId,
      int first = 25,
      string? after = null,
      string? queryFilter = null)
    {
        int pageSize = first;
        if (pageSize <= 0)
            pageSize = 25;
        ShopifyGraphQlClient client = CreateClient(storeDomain, tokenId);

        const string query = @"query ($first: Int!, $after: String, $query: String) {
  customers(first: $first, after: $after, query: $query) {
    pageInfo {
      hasNextPage
      endCursor
    }
    edges {
      node {
        id
        email
        firstName
        lastName
        phone
        tags
      }
    }
  }
}";

        GraphCustomersResult? result = client.ExecuteAsync<GraphCustomersResult>(query, new Dictionary<string, object>
        {
            ["first"] = pageSize,
            ["after"] = string.IsNullOrWhiteSpace(after) ? null! : after,
            ["query"] = queryFilter ?? string.Empty
        }).GetAwaiter().GetResult();

        ShopifyCustomer[] customers = result?.Customers?.Edges?
            .Where(edge => edge.Node != null)
            .Select(edge => ShopifyMapper.ToCustomer(edge.Node!))
            .ToArray() ?? Array.Empty<ShopifyCustomer>();

        return new ShopifyCustomersPage
        {
            Customers = customers,
            HasNextPage = result?.Customers?.PageInfo?.HasNextPage ?? false,
            EndCursor = result?.Customers?.PageInfo?.EndCursor ?? string.Empty
        };
    }

    private static ShopifyGraphQlClient CreateClient(string storeDomain, string tokenId)
    {
        (string normalizedStoreDomain, string accessToken) = ShopifyStepConnection.Resolve(storeDomain, tokenId);
        return new ShopifyGraphQlClient(normalizedStoreDomain, accessToken);
    }
}
