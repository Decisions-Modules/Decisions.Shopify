using Decisions.Shopify.Clients;
using Decisions.Shopify.Data;
using Decisions.Shopify.Utility;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.Design.Properties.Attributes;

namespace Decisions.Shopify.Steps;

[AutoRegisterMethodsOnClass(true, "Integration/Shopify/Inventory")]
public class InventorySteps
{
    public ShopifyVariant[] ListVariantsByProductId([PropertyClassification(0, "Store Domain", "Connection")] string storeDomain,
      [TokenPicker][PropertyClassification(0, "Token", "Connection")] string tokenId,
      string productId,
      int first = 50)
    {
        int pageSize = first;
        if (pageSize <= 0)
            pageSize = 50;

        ShopifyGraphQlClient client = CreateClient(storeDomain, tokenId);

        const string query = @"query ($id: ID!, $first: Int!) {
  product(id: $id) {
    variants(first: $first) {
      edges {
        node {
          id
          title
          sku
          price
          inventoryQuantity
          inventoryItem {
            id
          }
        }
      }
    }
  }
}";

        GraphVariantsForProductResult? result = client.ExecuteAsync<GraphVariantsForProductResult>(query, new Dictionary<string, object>
        {
            ["id"] = productId,
            ["first"] = pageSize
        }).GetAwaiter().GetResult();

        return result?.Product?.Variants?.Edges?
            .Where(edge => edge.Node != null)
            .Select(edge => ShopifyMapper.ToVariant(edge.Node!, productId))
            .ToArray() ?? Array.Empty<ShopifyVariant>();
    }

    public ShopifyMutationResult<ShopifyInventoryLevel> AdjustInventoryAvailable([PropertyClassification(0, "Store Domain", "Connection")] string storeDomain,
      [TokenPicker][PropertyClassification(0, "Token", "Connection")] string tokenId,
      string inventoryItemId,
      string locationId,
      int delta)
    {
        ShopifyGraphQlClient client = CreateClient(storeDomain, tokenId);

        const string mutation = @"mutation ($inventoryItemId: ID!, $locationId: ID!, $delta: Int!) {
  inventoryAdjustQuantities(input: {
    name: ""available"",
    reason: ""correction"",
    changes: [{
      inventoryItemId: $inventoryItemId,
      locationId: $locationId,
      delta: $delta
    }]
  }) {
    userErrors {
      field
      message
    }
  }
}";

        GraphInventoryAdjustResult? result = client.ExecuteAsync<GraphInventoryAdjustResult>(mutation, new Dictionary<string, object>
        {
            ["inventoryItemId"] = inventoryItemId,
            ["locationId"] = locationId,
            ["delta"] = delta
        }).GetAwaiter().GetResult();

        return new ShopifyMutationResult<ShopifyInventoryLevel>
        {
            Data = new ShopifyInventoryLevel
            {
                InventoryItemId = inventoryItemId,
                LocationId = locationId,
                Available = null
            },
            UserErrors = ShopifyMapper.ToUserErrors(result?.InventoryAdjustQuantities?.UserErrors)
        };
    }

    private static ShopifyGraphQlClient CreateClient(string storeDomain, string tokenId)
    {
        (string normalizedStoreDomain, string accessToken) = ShopifyStepConnection.Resolve(storeDomain, tokenId);
        return new ShopifyGraphQlClient(normalizedStoreDomain, accessToken);
    }
}
