using GovernAI.Abstractions;
using Microsoft.AspNetCore.Http;

namespace GovernAI.AspNetCore;

/// <summary>
/// Resolves the tenant ID from claims only.
/// Checks <c>tenant_id</c> and <c>tid</c> claims.
/// </summary>
public sealed class ClaimsTenantResolver : IGovernAITenantResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of <see cref="ClaimsTenantResolver"/>.
    /// </summary>
    public ClaimsTenantResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor
            ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <inheritdoc />
    public ValueTask<string?> ResolveTenantIdAsync(CancellationToken cancellationToken = default)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user is null)
        {
            return ValueTask.FromResult<string?>(null);
        }

        var tenantId = user.FindFirst("tenant_id")?.Value
                       ?? user.FindFirst("tid")?.Value;

        return ValueTask.FromResult(string.IsNullOrWhiteSpace(tenantId) ? null : tenantId);
    }
}
