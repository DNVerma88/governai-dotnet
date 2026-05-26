using System.Security.Claims;
using GovernAI.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace GovernAI.AspNetCore.Tests;

public sealed class TenantAndUserResolverTests
{
    private static HttpContextTenantResolver BuildTenantResolver(HttpContext httpContext)
    {
        var accessor = new MockHttpContextAccessor(httpContext);
        return new HttpContextTenantResolver(accessor);
    }

    private static HttpContextUserResolver BuildUserResolver(HttpContext httpContext)
    {
        var accessor = new MockHttpContextAccessor(httpContext);
        return new HttpContextUserResolver(accessor);
    }

    [Fact]
    public async Task TenantResolver_FromHeader_ReturnsTenantId()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Tenant-Id"] = "tenant-from-header";

        var resolver = BuildTenantResolver(context);
        var tenantId = await resolver.ResolveTenantIdAsync();

        Assert.Equal("tenant-from-header", tenantId);
    }

    [Fact]
    public async Task TenantResolver_FromClaim_ReturnsTenantId()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim("tenant_id", "claim-tenant")], "test"));

        var resolver = BuildTenantResolver(context);
        var tenantId = await resolver.ResolveTenantIdAsync();

        Assert.Equal("claim-tenant", tenantId);
    }

    [Fact]
    public async Task TenantResolver_ClaimsTakePriorityOverHeaderWhenAuthenticated()
    {
        // For authenticated users, JWT claims are the authoritative tenant source.
        // A client-supplied header must NOT override a claim-based tenant to prevent spoofing.
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Tenant-Id"] = "header-tenant";
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim("tenant_id", "claim-tenant")], "test"));

        var resolver = BuildTenantResolver(context);
        var tenantId = await resolver.ResolveTenantIdAsync();

        Assert.Equal("claim-tenant", tenantId);
    }

    [Fact]
    public async Task TenantResolver_HeaderUsedWhenAuthenticatedButNoTenantClaim()
    {
        // Authenticated request but JWT carries no tenant claim:
        // header is accepted as a fallback for systems that authenticate
        // without embedding tenant context in the token.
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Tenant-Id"] = "header-tenant";
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim("sub", "some-user")], "test")); // authenticated, but no tenant_id/tid claim

        var resolver = BuildTenantResolver(context);
        var tenantId = await resolver.ResolveTenantIdAsync();

        Assert.Equal("header-tenant", tenantId);
    }

    [Fact]
    public async Task TenantResolver_NoContext_ReturnsNull()
    {
        var accessor = new MockHttpContextAccessor(null);
        var resolver = new HttpContextTenantResolver(accessor);

        var tenantId = await resolver.ResolveTenantIdAsync();

        Assert.Null(tenantId);
    }

    [Fact]
    public async Task UserResolver_FromSubClaim_ReturnsUserId()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim("sub", "user-sub-id")], "test"));

        var resolver = BuildUserResolver(context);
        var userId = await resolver.ResolveUserIdAsync();

        Assert.Equal("user-sub-id", userId);
    }

    [Fact]
    public async Task UserResolver_FromNameIdentifierClaim_ReturnsUserId()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(System.Security.Claims.ClaimTypes.NameIdentifier, "name-id-user")], "test"));

        var resolver = BuildUserResolver(context);
        var userId = await resolver.ResolveUserIdAsync();

        Assert.Equal("name-id-user", userId);
    }

    [Fact]
    public async Task UserResolver_NoContext_ReturnsNull()
    {
        var accessor = new MockHttpContextAccessor(null);
        var resolver = new HttpContextUserResolver(accessor);

        var userId = await resolver.ResolveUserIdAsync();

        Assert.Null(userId);
    }

    private sealed class MockHttpContextAccessor : IHttpContextAccessor
    {
        public MockHttpContextAccessor(HttpContext? context) => HttpContext = context;
        public HttpContext? HttpContext { get; set; }
    }
}

public sealed class HeaderTenantResolverTests
{
    private static HeaderTenantResolver Build(HttpContext? ctx, string? headerName = null)
    {
        var accessor = new MockAccessor(ctx);
        return headerName is null
            ? new HeaderTenantResolver(accessor)
            : new HeaderTenantResolver(accessor, headerName);
    }

    [Fact]
    public async Task ResolveTenantIdAsync_DefaultHeader_ReturnsTenantId()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-Tenant-Id"] = "tenant-abc";

        var resolver = Build(context);
        var result = await resolver.ResolveTenantIdAsync();

        Assert.Equal("tenant-abc", result);
    }

    [Fact]
    public async Task ResolveTenantIdAsync_CustomHeader_ReturnsTenantId()
    {
        var context = new DefaultHttpContext();
        context.Request.Headers["X-My-Tenant"] = "my-tenant";

        var resolver = Build(context, "X-My-Tenant");
        var result = await resolver.ResolveTenantIdAsync();

        Assert.Equal("my-tenant", result);
    }

    [Fact]
    public async Task ResolveTenantIdAsync_HeaderMissing_ReturnsNull()
    {
        var resolver = Build(new DefaultHttpContext());
        var result = await resolver.ResolveTenantIdAsync();
        Assert.Null(result);
    }

    [Fact]
    public async Task ResolveTenantIdAsync_NoContext_ReturnsNull()
    {
        var resolver = Build(null);
        var result = await resolver.ResolveTenantIdAsync();
        Assert.Null(result);
    }

    [Fact]
    public void Constructor_NullAccessor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new HeaderTenantResolver(null!));
    }

    private sealed class MockAccessor : IHttpContextAccessor
    {
        public MockAccessor(HttpContext? ctx) => HttpContext = ctx;
        public HttpContext? HttpContext { get; set; }
    }
}

public sealed class ClaimsTenantResolverTests
{
    private static ClaimsTenantResolver Build(HttpContext? ctx)
    {
        var accessor = new MockAccessor(ctx);
        return new ClaimsTenantResolver(accessor);
    }

    [Fact]
    public async Task ResolveTenantIdAsync_TenantIdClaim_ReturnsTenant()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim("tenant_id", "claim-tenant")], "test"));

        var result = await Build(context).ResolveTenantIdAsync();

        Assert.Equal("claim-tenant", result);
    }

    [Fact]
    public async Task ResolveTenantIdAsync_TidClaim_ReturnsTenant()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim("tid", "tid-tenant")], "test"));

        var result = await Build(context).ResolveTenantIdAsync();

        Assert.Equal("tid-tenant", result);
    }

    [Fact]
    public async Task ResolveTenantIdAsync_NoClaims_ReturnsNull()
    {
        var result = await Build(new DefaultHttpContext()).ResolveTenantIdAsync();
        Assert.Null(result);
    }

    [Fact]
    public async Task ResolveTenantIdAsync_NoContext_ReturnsNull()
    {
        var result = await Build(null).ResolveTenantIdAsync();
        Assert.Null(result);
    }

    [Fact]
    public void Constructor_NullAccessor_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new ClaimsTenantResolver(null!));
    }

    private sealed class MockAccessor : IHttpContextAccessor
    {
        public MockAccessor(HttpContext? ctx) => HttpContext = ctx;
        public HttpContext? HttpContext { get; set; }
    }
}
