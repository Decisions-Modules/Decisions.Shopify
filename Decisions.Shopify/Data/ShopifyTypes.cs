using DecisionsFramework;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace Decisions.Shopify.Data;

[Writable]
public class ShopifyUserError
{
    [WritableValue]
    public string[] Field { get; set; } = Array.Empty<string>();
    [WritableValue]
    public string Message { get; set; } = string.Empty;
}

[Writable]
public class ShopifyProduct
{
    [WritableValue]
    public string Id { get; set; } = string.Empty;
    [WritableValue]
    public string Title { get; set; } = string.Empty;
    [WritableValue]
    public string Handle { get; set; } = string.Empty;
    [WritableValue]
    public string Status { get; set; } = string.Empty;
    [WritableValue]
    public string Vendor { get; set; } = string.Empty;
    [WritableValue]
    public string ProductType { get; set; } = string.Empty;
}

[Writable]
public class ShopifyProductInput
{
    [WritableValue]
    public string Title { get; set; } = string.Empty;
    [WritableValue]
    public string? Handle { get; set; }
    [WritableValue]
    public string? Status { get; set; }
    [WritableValue]
    public string? Vendor { get; set; }
    [WritableValue]
    public string? ProductType { get; set; }
}

[Writable]
public class ShopifyVariant
{
    [WritableValue]
    public string Id { get; set; } = string.Empty;
    [WritableValue]
    public string ProductId { get; set; } = string.Empty;
    [WritableValue]
    public string Title { get; set; } = string.Empty;
    [WritableValue]
    public string Sku { get; set; } = string.Empty;
    [WritableValue]
    public string Price { get; set; } = string.Empty;
    [WritableValue]
    public string InventoryItemId { get; set; } = string.Empty;
    [WritableValue]
    public int? InventoryQuantity { get; set; }
    [WritableValue]
    public ShopifyInventoryLevel[] InventoryLevels { get; set; } = Array.Empty<ShopifyInventoryLevel>();
}

[Writable]
public class ShopifyInventoryLevel
{
    [WritableValue]
    public string InventoryItemId { get; set; } = string.Empty;
    [WritableValue]
    public string LocationId { get; set; } = string.Empty;
    [WritableValue]
    public int? Available { get; set; }
}

[Writable]
public class ShopifyLocation
{
    [WritableValue]
    public string Id { get; set; } = string.Empty;
    [WritableValue]
    public string Name { get; set; } = string.Empty;
    [WritableValue]
    public bool? IsActive { get; set; }
}

[Writable]
public class ShopifyOrder
{
    [WritableValue]
    public string Id { get; set; } = string.Empty;
    [WritableValue]
    public string CustomerId { get; set; } = string.Empty;
    [WritableValue]
    public string Name { get; set; } = string.Empty;
    [WritableValue]
    public string Email { get; set; } = string.Empty;
    [WritableValue]
    public string FinancialStatus { get; set; } = string.Empty;
    [WritableValue]
    public string FulfillmentStatus { get; set; } = string.Empty;
    [WritableValue]
    public string TotalPrice { get; set; } = string.Empty;
    [WritableValue]
    public string CurrencyCode { get; set; } = string.Empty;
    [WritableValue]
    public DateTimeOffset? CreatedAt { get; set; }
}

[Writable]
public class ShopifyCustomer
{
    [WritableValue]
    public string Id { get; set; } = string.Empty;
    [WritableValue]
    public string Email { get; set; } = string.Empty;
    [WritableValue]
    public string FirstName { get; set; } = string.Empty;
    [WritableValue]
    public string LastName { get; set; } = string.Empty;
    [WritableValue]
    public string Phone { get; set; } = string.Empty;
    [WritableValue]
    public string Tags { get; set; } = string.Empty;
}

[Writable]
public class ShopifyProductsPage
{
    [WritableValue]
    public ShopifyProduct[] Products { get; set; } = Array.Empty<ShopifyProduct>();
    [WritableValue]
    public bool HasNextPage { get; set; }
    [WritableValue]
    public string EndCursor { get; set; } = string.Empty;
}

[Writable]
public class ShopifyOrdersPage
{
    [WritableValue]
    public ShopifyOrder[] Orders { get; set; } = Array.Empty<ShopifyOrder>();
    [WritableValue]
    public bool HasNextPage { get; set; }
    [WritableValue]
    public string EndCursor { get; set; } = string.Empty;
}

[Writable]
public class ShopifyCustomersPage
{
    [WritableValue]
    public ShopifyCustomer[] Customers { get; set; } = Array.Empty<ShopifyCustomer>();
    [WritableValue]
    public bool HasNextPage { get; set; }
    [WritableValue]
    public string EndCursor { get; set; } = string.Empty;
}

[Writable]
public class ShopifyMutationResult<T>
{
    [WritableValue]
    public T? Data { get; set; }
    [WritableValue]
    public ShopifyUserError[] UserErrors { get; set; } = Array.Empty<ShopifyUserError>();
}
