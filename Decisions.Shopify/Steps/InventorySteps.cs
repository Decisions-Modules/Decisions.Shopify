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
            inventoryLevels(first: 50) {
              edges {
                node {
                  quantities(names: [""available""]) {
                    name
                    quantity
                  }
                  item {
                    id
                  }
                  location {
                    id
                  }
                }
              }
            }
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

    public ShopifyLocation[] ListLocations([PropertyClassification(0, "Store Domain", "Connection")] string storeDomain,
      [TokenPicker][PropertyClassification(0, "Token", "Connection")] string tokenId,
      int first = 50,
      string? after = null)
    {
        int pageSize = first;
        if (pageSize <= 0)
            pageSize = 50;

        ShopifyGraphQlClient client = CreateClient(storeDomain, tokenId);

        const string query = @"query ($first: Int!, $after: String) {
  locations(first: $first, after: $after) {
    edges {
      node {
        id
        name
        isActive
      }
    }
  }
}";

        GraphLocationsResult? result = client.ExecuteAsync<GraphLocationsResult>(query, new Dictionary<string, object>
        {
            ["first"] = pageSize,
            ["after"] = string.IsNullOrWhiteSpace(after) ? null! : after
        }).GetAwaiter().GetResult();

        return result?.Locations?.Edges?
            .Where(edge => edge.Node != null)
            .Select(edge => ShopifyMapper.ToLocation(edge.Node!))
            .ToArray() ?? Array.Empty<ShopifyLocation>();
    }

    public ShopifyInventoryLevel[] ListInventoryLevelsByInventoryItemId([PropertyClassification(0, "Store Domain", "Connection")] string storeDomain,
      [TokenPicker][PropertyClassification(0, "Token", "Connection")] string tokenId,
      string inventoryItemId,
      int first = 50)
    {
        int pageSize = first;
        if (pageSize <= 0)
            pageSize = 50;

        ShopifyGraphQlClient client = CreateClient(storeDomain, tokenId);

        const string query = @"query ($id: ID!, $first: Int!) {
  inventoryItem(id: $id) {
    inventoryLevels(first: $first) {
      edges {
        node {
          quantities(names: [""available""]) {
            name
            quantity
          }
          item {
            id
          }
          location {
            id
          }
        }
      }
    }
  }
}";

        GraphInventoryLevelsForItemResult? result = client.ExecuteAsync<GraphInventoryLevelsForItemResult>(query, new Dictionary<string, object>
        {
            ["id"] = inventoryItemId,
            ["first"] = pageSize
        }).GetAwaiter().GetResult();

        return result?.InventoryItem?.InventoryLevels?.Edges?
            .Where(edge => edge.Node != null)
            .Select(edge => ShopifyMapper.ToInventoryLevel(edge.Node!))
            .ToArray() ?? Array.Empty<ShopifyInventoryLevel>();
    }

    private static ShopifyGraphQlClient CreateClient(string storeDomain, string tokenId)
    {
        (string normalizedStoreDomain, string accessToken) = ShopifyStepConnection.Resolve(storeDomain, tokenId);
        return new ShopifyGraphQlClient(normalizedStoreDomain, accessToken);
    }
}
