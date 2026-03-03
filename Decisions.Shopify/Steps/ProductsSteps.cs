using Decisions.Shopify.Clients;
using Decisions.Shopify.Data;
using Decisions.Shopify.Utility;
using DecisionsFramework;
using DecisionsFramework.Design.Flow;
using DecisionsFramework.Design.Properties;
using DecisionsFramework.Design.Properties.Attributes;

namespace Decisions.Shopify.Steps;

[AutoRegisterMethodsOnClass(true, "Integration/Shopify/Products")]
public class ProductsSteps
{
    public ShopifyProduct? GetProductById([PropertyClassification(0, "Store Domain", "Connection")] string storeDomain,
      [TokenPicker][PropertyClassification(0, "Token", "Connection")] string tokenId,
      string productId)
    {
        ShopifyGraphQlClient client = CreateClient(storeDomain, tokenId);

        const string query = @"query ($id: ID!) {
  product(id: $id) {
    id
    title
    handle
    status
    vendor
    productType
  }
}";

        GraphProductResult? result = client.ExecuteAsync<GraphProductResult>(query, new Dictionary<string, object>
        {
            ["id"] = productId
        }).GetAwaiter().GetResult();

        return result?.Product == null ? null : ShopifyMapper.ToProduct(result.Product);
    }

    public ShopifyProductsPage ListProducts([PropertyClassification(0, "Store Domain", "Connection")] string storeDomain,
      [TokenPicker][PropertyClassification(0, "Token", "Connection")] string tokenId,
      int first = 25,
      string? after = null)
    {
        int pageSize = first;
        if (pageSize <= 0)
            pageSize = 25;
        ShopifyGraphQlClient client = CreateClient(storeDomain, tokenId);

        const string query = @"query ($first: Int!, $after: String) {
  products(first: $first, after: $after) {
    pageInfo {
      hasNextPage
      endCursor
    }
    edges {
      node {
        id
        title
        handle
        status
        vendor
        productType
      }
    }
  }
}";

        GraphProductsResult? result = client.ExecuteAsync<GraphProductsResult>(query, new Dictionary<string, object>
        {
            ["first"] = pageSize,
            ["after"] = string.IsNullOrWhiteSpace(after) ? null! : after
        }).GetAwaiter().GetResult();

        ShopifyProduct[] products = result?.Products?.Edges?
            .Where(edge => edge.Node != null)
            .Select(edge => ShopifyMapper.ToProduct(edge.Node!))
            .ToArray() ?? Array.Empty<ShopifyProduct>();

        return new ShopifyProductsPage
        {
            Products = products,
            HasNextPage = result?.Products?.PageInfo?.HasNextPage ?? false,
            EndCursor = result?.Products?.PageInfo?.EndCursor ?? string.Empty
        };
    }

    public ShopifyMutationResult<ShopifyProduct> CreateProduct([PropertyClassification(0, "Store Domain", "Connection")] string storeDomain,
      [TokenPicker][PropertyClassification(0, "Token", "Connection")] string tokenId,
      ShopifyProductInput product)
    {
        if (string.IsNullOrWhiteSpace(product?.Title))
            throw new BusinessRuleException("Product title is required.");

        ShopifyGraphQlClient client = CreateClient(storeDomain, tokenId);

        const string mutation = @"mutation ($product: ProductCreateInput!) {
  productCreate(product: $product) {
    product {
      id
      title
      handle
      status
      vendor
      productType
    }
    userErrors {
      field
      message
    }
  }
}";

        Dictionary<string, object?> productInput = new()
        {
          ["title"] = product.Title,
          ["handle"] = string.IsNullOrWhiteSpace(product.Handle) ? null : product.Handle,
          ["status"] = string.IsNullOrWhiteSpace(product.Status) ? null : product.Status,
          ["vendor"] = string.IsNullOrWhiteSpace(product.Vendor) ? null : product.Vendor,
          ["productType"] = string.IsNullOrWhiteSpace(product.ProductType) ? null : product.ProductType
        };

        GraphProductCreateResult? result = client.ExecuteAsync<GraphProductCreateResult>(mutation, new Dictionary<string, object>
        {
            ["product"] = productInput
        }).GetAwaiter().GetResult();

        return new ShopifyMutationResult<ShopifyProduct>
        {
            Data = result?.ProductCreate?.Product == null ? null : ShopifyMapper.ToProduct(result.ProductCreate.Product),
            UserErrors = ShopifyMapper.ToUserErrors(result?.ProductCreate?.UserErrors)
        };
    }

    public ShopifyMutationResult<ShopifyProduct> UpdateProduct([PropertyClassification(0, "Store Domain", "Connection")] string storeDomain,
      [TokenPicker][PropertyClassification(0, "Token", "Connection")] string tokenId,
      string productId,
      ShopifyProductInput product)
    {
        if (product == null)
            throw new BusinessRuleException("Product input is required.");

        ShopifyGraphQlClient client = CreateClient(storeDomain, tokenId);

        const string mutation = @"mutation ($product: ProductUpdateInput!) {
  productUpdate(product: $product) {
    product {
      id
      title
      handle
      status
      vendor
      productType
    }
    userErrors {
      field
      message
    }
  }
}";

        Dictionary<string, object?> productInput = new()
        {
          ["id"] = productId,
          ["title"] = string.IsNullOrWhiteSpace(product.Title) ? null : product.Title,
          ["handle"] = string.IsNullOrWhiteSpace(product.Handle) ? null : product.Handle,
          ["status"] = string.IsNullOrWhiteSpace(product.Status) ? null : product.Status,
          ["vendor"] = string.IsNullOrWhiteSpace(product.Vendor) ? null : product.Vendor,
          ["productType"] = string.IsNullOrWhiteSpace(product.ProductType) ? null : product.ProductType
        };

        GraphProductUpdateResult? result = client.ExecuteAsync<GraphProductUpdateResult>(mutation, new Dictionary<string, object>
        {
            ["product"] = productInput
        }).GetAwaiter().GetResult();

        return new ShopifyMutationResult<ShopifyProduct>
        {
            Data = result?.ProductUpdate?.Product == null ? null : ShopifyMapper.ToProduct(result.ProductUpdate.Product),
            UserErrors = ShopifyMapper.ToUserErrors(result?.ProductUpdate?.UserErrors)
        };
    }

    public ShopifyMutationResult<bool> DeleteProduct([PropertyClassification(0, "Store Domain", "Connection")] string storeDomain,
      [TokenPicker][PropertyClassification(0, "Token", "Connection")] string tokenId,
      string productId)
    {
        ShopifyGraphQlClient client = CreateClient(storeDomain, tokenId);

        const string mutation = @"mutation ($id: ID!) {
  productDelete(input: { id: $id }) {
    deletedProductId
    userErrors {
      field
      message
    }
  }
}";

        GraphProductDeleteResult? result = client.ExecuteAsync<GraphProductDeleteResult>(mutation, new Dictionary<string, object>
        {
            ["id"] = productId
        }).GetAwaiter().GetResult();

        return new ShopifyMutationResult<bool>
        {
            Data = !string.IsNullOrWhiteSpace(result?.ProductDelete?.DeletedProductId),
            UserErrors = ShopifyMapper.ToUserErrors(result?.ProductDelete?.UserErrors)
        };
    }

    private static ShopifyGraphQlClient CreateClient(string storeDomain, string tokenId)
    {
        (string normalizedStoreDomain, string accessToken) = ShopifyStepConnection.Resolve(storeDomain, tokenId);
        return new ShopifyGraphQlClient(normalizedStoreDomain, accessToken);
    }
}
