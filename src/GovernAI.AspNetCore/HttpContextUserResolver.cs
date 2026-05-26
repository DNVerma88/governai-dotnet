using System.Security.Claims;
using GovernAI.Abstractions;
using Microsoft.AspNetCore.Http;

namespace GovernAI.AspNetCore;

/// <summary>
/// Resolves the user ID from the HTTP context claims.
/// Checks <c>sub</c>, <c>nameidentifier</c>, and <c>email</c> claims in order.
/// </summary>
public sealed class HttpContextUserResolver : IGovernAIUserResolver
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of <see cref="HttpContextUserResolver"/>.
    /// </summary>
    public HttpContextUserResolver(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor
            ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <inheritdoc />
    public ValueTask<string?> ResolveUserIdAsync(CancellationToken cancellationToken = default)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user is null)
        {
            return ValueTask.FromResult<string?>(null);
        }

        // 1. sub claim (OIDC standard)
        var sub = user.FindFirst("sub")?.Value;
        if (!string.IsNullOrWhiteSpace(sub))
        {
            return ValueTask.FromResult<string?>(sub);
        }

        // 2. nameidentifier claim
        var nameId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrWhiteSpace(nameId))
        {
            return ValueTask.FromResult<string?>(nameId);
        }

        // 3. email claim
        var email = user.FindFirst(ClaimTypes.Email)?.Value
                    ?? user.FindFirst("email")?.Value;
        if (!string.IsNullOrWhiteSpace(email))
        {
            return ValueTask.FromResult<string?>(email);
        }

        return ValueTask.FromResult<string?>(null);
    }
}
