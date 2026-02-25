using DecisionsFramework;
using DecisionsFramework.Design.ConfigurationStorage.Attributes;

namespace Decisions.Shopify.Data;

[Writable]
public class ShopifyUserError
{
    public string[] Field { get; set; } = Array.Empty<string>();
    public string Message { get; set; } = string.Empty;
}

[Writable]
public class ShopifyProduct
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Handle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Vendor { get; set; } = string.Empty;
    public string ProductType { get; set; } = string.Empty;
}

[Writable]
public class ShopifyProductInput
{
    public string Title { get; set; } = string.Empty;
    public string? Handle { get; set; }
    public string? Status { get; set; }
    public string? Vendor { get; set; }
    public string? ProductType { get; set; }
}

[Writable]
public class ShopifyVariant
{
    public string Id { get; set; } = string.Empty;
    public string ProductId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
    public string InventoryItemId { get; set; } = string.Empty;
    public int? InventoryQuantity { get; set; }
}

[Writable]
public class ShopifyInventoryLevel
{
    public string InventoryItemId { get; set; } = string.Empty;
    public string LocationId { get; set; } = string.Empty;
    public int? Available { get; set; }
}

[Writable]
public class ShopifyOrder
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FinancialStatus { get; set; } = string.Empty;
    public string FulfillmentStatus { get; set; } = string.Empty;
    public string TotalPrice { get; set; } = string.Empty;
    public string CurrencyCode { get; set; } = string.Empty;
    public DateTimeOffset? CreatedAt { get; set; }
}

[Writable]
public class ShopifyCustomer
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
}

[Writable]
public class ShopifyProductsPage
{
    public ShopifyProduct[] Products { get; set; } = Array.Empty<ShopifyProduct>();
    public bool HasNextPage { get; set; }
    public string EndCursor { get; set; } = string.Empty;
}

[Writable]
public class ShopifyOrdersPage
{
    public ShopifyOrder[] Orders { get; set; } = Array.Empty<ShopifyOrder>();
    public bool HasNextPage { get; set; }
    public string EndCursor { get; set; } = string.Empty;
}

[Writable]
public class ShopifyCustomersPage
{
    public ShopifyCustomer[] Customers { get; set; } = Array.Empty<ShopifyCustomer>();
    public bool HasNextPage { get; set; }
    public string EndCursor { get; set; } = string.Empty;
}

[Writable]
public class ShopifyMutationResult<T>
{
    public T? Data { get; set; }
    public ShopifyUserError[] UserErrors { get; set; } = Array.Empty<ShopifyUserError>();
}
