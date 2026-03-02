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
        ShopifyInventoryLevel[] inventoryLevels = node.InventoryItem?.InventoryLevels?.Edges?
            .Where(edge => edge.Node != null)
            .Select(edge => ToInventoryLevel(edge.Node!))
            .ToArray() ?? Array.Empty<ShopifyInventoryLevel>();

        return new ShopifyVariant
        {
            Id = node.Id ?? string.Empty,
            ProductId = productId,
            Title = node.Title ?? string.Empty,
            Sku = node.Sku ?? string.Empty,
            Price = node.Price ?? string.Empty,
            InventoryQuantity = node.InventoryQuantity,
            InventoryItemId = node.InventoryItem?.Id ?? string.Empty,
            InventoryLevels = inventoryLevels
        };
    }

    public static ShopifyInventoryLevel ToInventoryLevel(GraphInventoryLevelNode node)
    {
        int? available = node.Quantities?
            .FirstOrDefault(quantity => string.Equals(quantity.Name, "available", StringComparison.OrdinalIgnoreCase))?
            .Quantity ?? node.Quantities?.FirstOrDefault()?.Quantity;

        return new ShopifyInventoryLevel
        {
            Available = available,
            InventoryItemId = node.Item?.Id ?? string.Empty,
            LocationId = node.Location?.Id ?? string.Empty
        };
    }

    public static ShopifyLocation ToLocation(GraphLocationNode node)
    {
        return new ShopifyLocation
        {
            Id = node.Id ?? string.Empty,
            Name = node.Name ?? string.Empty,
            IsActive = node.IsActive
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
        string email = node.DefaultEmailAddress?.EmailAddress ?? node.Email ?? string.Empty;
        string phone = node.DefaultPhoneNumber?.PhoneNumber ?? node.Phone ?? string.Empty;

        string tags = node.Tags == null
            ? string.Empty
            : string.Join(", ", node.Tags.Where(tag => !string.IsNullOrWhiteSpace(tag)));

        return new ShopifyCustomer
        {
            Id = node.Id ?? string.Empty,
            Email = email,
            FirstName = node.FirstName ?? string.Empty,
            LastName = node.LastName ?? string.Empty,
            Phone = phone,
            Tags = tags
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
