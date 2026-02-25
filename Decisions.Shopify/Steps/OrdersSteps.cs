using Decisions.Shopify.Clients;
using Decisions.Shopify.Data;
using Decisions.Shopify.Utility;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.Design.Properties.Attributes;

namespace Decisions.Shopify.Steps;

[AutoRegisterMethodsOnClass(true, "Integration/Shopify/Orders")]
public class OrdersSteps
{
    public ShopifyOrder? GetOrderById([PropertyClassification(0, "Store Domain", "Connection")] string storeDomain,
      [TokenPicker][PropertyClassification(0, "Token", "Connection")] string tokenId,
      string orderId)
    {
        ShopifyGraphQlClient client = CreateClient(storeDomain, tokenId);

        const string query = @"query ($id: ID!) {
  order(id: $id) {
    id
    name
    email
    displayFinancialStatus
    displayFulfillmentStatus
    createdAt
    totalPriceSet {
      shopMoney {
        amount
        currencyCode
      }
    }
  }
}";

        GraphOrderResult? result = client.ExecuteAsync<GraphOrderResult>(query, new Dictionary<string, object>
        {
            ["id"] = orderId
        }).GetAwaiter().GetResult();

        return result?.Order == null ? null : ShopifyMapper.ToOrder(result.Order);
    }

    public ShopifyOrdersPage ListOrders([PropertyClassification(0, "Store Domain", "Connection")] string storeDomain,
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
  orders(first: $first, after: $after, query: $query) {
    pageInfo {
      hasNextPage
      endCursor
    }
    edges {
      node {
        id
        name
        email
        displayFinancialStatus
        displayFulfillmentStatus
        createdAt
        totalPriceSet {
          shopMoney {
            amount
            currencyCode
          }
        }
      }
    }
  }
}";

        GraphOrdersResult? result = client.ExecuteAsync<GraphOrdersResult>(query, new Dictionary<string, object>
        {
            ["first"] = pageSize,
            ["after"] = string.IsNullOrWhiteSpace(after) ? null! : after,
            ["query"] = queryFilter ?? string.Empty
        }).GetAwaiter().GetResult();

        ShopifyOrder[] orders = result?.Orders?.Edges?
            .Where(edge => edge.Node != null)
            .Select(edge => ShopifyMapper.ToOrder(edge.Node!))
            .ToArray() ?? Array.Empty<ShopifyOrder>();

        return new ShopifyOrdersPage
        {
            Orders = orders,
            HasNextPage = result?.Orders?.PageInfo?.HasNextPage ?? false,
            EndCursor = result?.Orders?.PageInfo?.EndCursor ?? string.Empty
        };
    }

    private static ShopifyGraphQlClient CreateClient(string storeDomain, string tokenId)
    {
        (string normalizedStoreDomain, string accessToken) = ShopifyStepConnection.Resolve(storeDomain, tokenId);
        return new ShopifyGraphQlClient(normalizedStoreDomain, accessToken);
    }
}
