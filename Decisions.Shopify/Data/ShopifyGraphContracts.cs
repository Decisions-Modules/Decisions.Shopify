using System.Text.Json;
using System.Text.Json.Serialization;

namespace Decisions.Shopify.Data;

internal class GraphPageInfo
{
    public bool HasNextPage { get; set; }
    public string? EndCursor { get; set; }
}

internal class GraphEdge<T>
{
    public T? Node { get; set; }
}

internal class GraphConnection<T>
{
    public GraphPageInfo? PageInfo { get; set; }
    public List<GraphEdge<T>>? Edges { get; set; }
}

internal class GraphProductNode
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Handle { get; set; }
    public string? Status { get; set; }
    public string? Vendor { get; set; }
    public string? ProductType { get; set; }
}

internal class GraphVariantNode
{
    public string? Id { get; set; }
    public string? Title { get; set; }
    public string? Sku { get; set; }
    public string? Price { get; set; }
    public int? InventoryQuantity { get; set; }
    public GraphInventoryItem? InventoryItem { get; set; }
    public GraphConnection<GraphInventoryLevelNode>? InventoryLevels { get; set; }
}

internal class GraphInventoryItem
{
    public string? Id { get; set; }
}

internal class GraphInventoryLevelNode
{
    public int? Available { get; set; }
    public GraphInventoryItem? Item { get; set; }
    public GraphLocationNode? Location { get; set; }
}

internal class GraphLocationNode
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public bool? IsActive { get; set; }
}

internal class GraphOrderNode
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? DisplayFinancialStatus { get; set; }
    public string? DisplayFulfillmentStatus { get; set; }
    public DateTimeOffset? CreatedAt { get; set; }
    public GraphMoneyBag? TotalPriceSet { get; set; }
}

internal class GraphMoneyBag
{
    public GraphMoney? ShopMoney { get; set; }
}

internal class GraphMoney
{
    public string? Amount { get; set; }
    public string? CurrencyCode { get; set; }
}

internal class GraphCustomerNode
{
    public string? Id { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    [JsonConverter(typeof(StringOrStringArrayConverter))]
    public string[]? Tags { get; set; }
}

internal sealed class StringOrStringArrayConverter : JsonConverter<string[]?>
{
    public override string[]? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
            return null;

        if (reader.TokenType == JsonTokenType.String)
        {
            string? value = reader.GetString();
            return string.IsNullOrWhiteSpace(value) ? Array.Empty<string>() : new[] { value };
        }

        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException($"Unexpected token {reader.TokenType} for tags.");

        List<string> tags = new();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
                return tags.ToArray();

            if (reader.TokenType == JsonTokenType.String)
            {
                string? tag = reader.GetString();
                if (!string.IsNullOrWhiteSpace(tag))
                    tags.Add(tag);
            }
        }

        throw new JsonException("Incomplete JSON array for tags.");
    }

    public override void Write(Utf8JsonWriter writer, string[]? value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();
        foreach (string tag in value)
            writer.WriteStringValue(tag);
        writer.WriteEndArray();
    }
}

internal class GraphUserError
{
    public string[]? Field { get; set; }
    public string? Message { get; set; }
}

internal class GraphProductsResult
{
    public GraphConnection<GraphProductNode>? Products { get; set; }
}

internal class GraphProductResult
{
    public GraphProductNode? Product { get; set; }
}

internal class GraphVariantsForProductResult
{
    public GraphProductVariantsNode? Product { get; set; }
}

internal class GraphLocationsResult
{
    public GraphConnection<GraphLocationNode>? Locations { get; set; }
}

internal class GraphInventoryLevelsForItemResult
{
    public GraphInventoryItemLevelsNode? InventoryItem { get; set; }
}

internal class GraphInventoryItemLevelsNode
{
    public GraphConnection<GraphInventoryLevelNode>? InventoryLevels { get; set; }
}

internal class GraphProductVariantsNode
{
    public GraphConnection<GraphVariantNode>? Variants { get; set; }
}

internal class GraphOrdersResult
{
    public GraphConnection<GraphOrderNode>? Orders { get; set; }
}

internal class GraphOrderResult
{
    public GraphOrderNode? Order { get; set; }
}

internal class GraphCustomersResult
{
    public GraphConnection<GraphCustomerNode>? Customers { get; set; }
}

internal class GraphCustomerResult
{
    public GraphCustomerNode? Customer { get; set; }
}

internal class GraphProductCreateResult
{
    public GraphProductCreatePayload? ProductCreate { get; set; }
}

internal class GraphProductCreatePayload
{
    public GraphProductNode? Product { get; set; }
    public GraphUserError[]? UserErrors { get; set; }
}

internal class GraphProductUpdateResult
{
    public GraphProductUpdatePayload? ProductUpdate { get; set; }
}

internal class GraphProductUpdatePayload
{
    public GraphProductNode? Product { get; set; }
    public GraphUserError[]? UserErrors { get; set; }
}

internal class GraphProductDeleteResult
{
    public GraphProductDeletePayload? ProductDelete { get; set; }
}

internal class GraphProductDeletePayload
{
    public string? DeletedProductId { get; set; }
    public GraphUserError[]? UserErrors { get; set; }
}

internal class GraphInventoryAdjustResult
{
    public GraphInventoryAdjustPayload? InventoryAdjustQuantities { get; set; }
}

internal class GraphInventoryAdjustPayload
{
    public GraphUserError[]? UserErrors { get; set; }
}
