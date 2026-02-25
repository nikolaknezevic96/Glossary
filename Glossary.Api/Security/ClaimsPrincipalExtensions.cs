using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Glossary.Api.Security;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var sub = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
        if (sub is null || !Guid.TryParse(sub, out var id))
            throw new InvalidOperationException("User id claim (sub) is missing.");
        return id;
    }
}
