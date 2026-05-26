using GovernAI.Abstractions;
using Microsoft.AspNetCore.Http;

namespace GovernAI.AspNetCore;

/// <summary>
/// Resolves the tenant ID from the <c>X-Tenant-Id</c> request header only.
/// </summary>
public sealed class HeaderTenantResolver : IGovernAITenantResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _headerName;

    /// <summary>
    /// Initializes a new instance of <see cref="HeaderTenantResolver"/>.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor.</param>
    /// <param name="headerName">The header name to read. Defaults to <c>X-Tenant-Id</c>.</param>
    public HeaderTenantResolver(IHttpContextAccessor httpContextAccessor, string headerName = "X-Tenant-Id")
    {
        _httpContextAccessor = httpContextAccessor
            ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _headerName = string.IsNullOrWhiteSpace(headerName) ? "X-Tenant-Id" : headerName;
    }

    /// <inheritdoc />
    public ValueTask<string?> ResolveTenantIdAsync(CancellationToken cancellationToken = default)
    {
        var value = _httpContextAccessor.HttpContext?.Request.Headers[_headerName].FirstOrDefault();
        return ValueTask.FromResult(string.IsNullOrWhiteSpace(value) ? null : value);
    }
}
