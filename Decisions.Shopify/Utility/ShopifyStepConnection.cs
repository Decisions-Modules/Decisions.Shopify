using Decisions.OAuth;
using DecisionsFramework;
using DecisionsFramework.Data.ORMapper;

namespace Decisions.Shopify.Utility;

public static class ShopifyStepConnection
{
    public static (string StoreDomain, string AccessToken) Resolve(string storeDomain, string tokenId)
    {
        string normalizedStoreDomain = NormalizeStoreDomain(storeDomain);

        if (string.IsNullOrWhiteSpace(tokenId))
            throw new BusinessRuleException("Token is required.");

        ORM<OAuthToken> orm = new();
        OAuthToken? token = orm.Fetch(tokenId);

        if (token == null)
            throw new EntityNotFoundException($"Cannot find token with TokenId=\"{tokenId}\".");

        if (string.IsNullOrWhiteSpace(token.TokenData))
            throw new LoggedException($"Token entity '{token.EntityName}' does not contain an access token in TokenData.");

        return (normalizedStoreDomain, token.TokenData);
    }

    private static string NormalizeStoreDomain(string storeDomain)
    {
        string selected = storeDomain?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(selected))
            throw new BusinessRuleException("Store Domain is required.");

        selected = selected.Replace("https://", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("http://", string.Empty, StringComparison.OrdinalIgnoreCase)
            .TrimEnd('/');

        return selected;
    }
}
