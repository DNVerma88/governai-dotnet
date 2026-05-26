using GovernAI.Abstractions;
using Microsoft.AspNetCore.Http;

namespace GovernAI.AspNetCore;

/// <summary>
/// Resolves the tenant ID from the HTTP request context.
/// Checks the <c>X-Tenant-Id</c> request header first, then falls back to
/// <c>tenant_id</c> or <c>tid</c> claims.
/// </summary>
public sealed class HttpContextTenantResolver : IGovernAITenantResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of <see cref="HttpContextTenantResolver"/>.
    /// </summary>
    public HttpContextTenantResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor
            ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <inheritdoc />
    public ValueTask<string?> ResolveTenantIdAsync(CancellationToken cancellationToken = default)
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is null)
        {
            return ValueTask.FromResult<string?>(null);
        }

        // For authenticated users, claims are the authoritative source of tenant identity.
        // Trusting a client-supplied header over claims allows any caller to spoof a tenant
        // ID and poison the audit trail or bypass tenant-scoped policies (VULN-01).
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            // 1a. Claims: tenant_id claim
            var tenantIdClaim = context.User.FindFirst("tenant_id")?.Value;
            if (!string.IsNullOrWhiteSpace(tenantIdClaim))
            {
                return ValueTask.FromResult<string?>(tenantIdClaim);
            }

            // 1b. Claims: tid claim (Azure AD)
            var tidClaim = context.User.FindFirst("tid")?.Value;
            if (!string.IsNullOrWhiteSpace(tidClaim))
            {
                return ValueTask.FromResult<string?>(tidClaim);
            }
        }

        // 2. Header fallback: accepted for unauthenticated M2M / gateway-forwarded requests
        //    where the tenant is injected by a trusted upstream proxy rather than a JWT.
        //    When the user IS authenticated but carries no tenant claim, the header is also
        //    used so that systems that authenticate without tenant context still function.
        var headerValue = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(headerValue))
        {
            return ValueTask.FromResult<string?>(headerValue);
        }

        return ValueTask.FromResult<string?>(null);
    }
}
