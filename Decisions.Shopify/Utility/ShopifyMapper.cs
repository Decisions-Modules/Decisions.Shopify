using Decisions.Shopify.Data;

namespace Decisions.Shopify.Utility;

internal static class ShopifyMapper
{
    public static ShopifyProduct ToProduct(GraphProductNode node)
    {
        return new ShopifyProduct
        {
            Id = node.Id ?? string.Empty,
            Title = node.Title ?? string.Empty,
            Handle = node.Handle ?? string.Empty,
            Status = node.Status ?? string.Empty,
            Vendor = node.Vendor ?? string.Empty,
            ProductType = node.ProductType ?? string.Empty
        };
    }

    public static ShopifyVariant ToVariant(GraphVariantNode node, string productId)
    {
        return new ShopifyVariant
        {
            Id = node.Id ?? string.Empty,
            ProductId = productId,
            Title = node.Title ?? string.Empty,
            Sku = node.Sku ?? string.Empty,
            Price = node.Price ?? string.Empty,
            InventoryQuantity = node.InventoryQuantity,
            InventoryItemId = node.InventoryItem?.Id ?? string.Empty
        };
    }

    public static ShopifyInventoryLevel ToInventoryLevel(GraphInventoryLevelNode node)
    {
        return new ShopifyInventoryLevel
        {
            Available = node.Available,
            InventoryItemId = node.Item?.Id ?? string.Empty,
            LocationId = node.Location?.Id ?? string.Empty
        };
    }

    public static ShopifyOrder ToOrder(GraphOrderNode node)
    {
        return new ShopifyOrder
        {
            Id = node.Id ?? string.Empty,
            Name = node.Name ?? string.Empty,
            Email = node.Email ?? string.Empty,
            FinancialStatus = node.DisplayFinancialStatus ?? string.Empty,
            FulfillmentStatus = node.DisplayFulfillmentStatus ?? string.Empty,
            TotalPrice = node.TotalPriceSet?.ShopMoney?.Amount ?? string.Empty,
            CurrencyCode = node.TotalPriceSet?.ShopMoney?.CurrencyCode ?? string.Empty,
            CreatedAt = node.CreatedAt
        };
    }

    public static ShopifyCustomer ToCustomer(GraphCustomerNode node)
    {
        return new ShopifyCustomer
        {
            Id = node.Id ?? string.Empty,
            Email = node.Email ?? string.Empty,
            FirstName = node.FirstName ?? string.Empty,
            LastName = node.LastName ?? string.Empty,
            Phone = node.Phone ?? string.Empty,
            Tags = node.Tags ?? string.Empty
        };
    }

    public static ShopifyUserError[] ToUserErrors(GraphUserError[]? userErrors)
    {
        if (userErrors == null || userErrors.Length == 0)
            return Array.Empty<ShopifyUserError>();

        return userErrors.Select(error => new ShopifyUserError
        {
            Field = error.Field ?? Array.Empty<string>(),
            Message = error.Message ?? string.Empty
        }).ToArray();
    }
}
