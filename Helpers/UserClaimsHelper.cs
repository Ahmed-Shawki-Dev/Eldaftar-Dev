using System.Security.Claims;

namespace api.Helpers;

public record UserContext(Guid TenantId, Guid TeacherId, string TenantSlug);

public static class UserClaimsExtensions
{
    // Extract and return UserContext from Claims or null if invalid
    public static UserContext? GetUserContext(this ClaimsPrincipal user)
    {
        var tenantClaim = user.FindFirst("tenantId")?.Value;
        var teacherClaim = user.FindFirst("teacherId")?.Value;
        var slugClaim = user.FindFirst("tenantSlug")?.Value;

        if (
            Guid.TryParse(tenantClaim, out var tenantId)
            && Guid.TryParse(teacherClaim, out var teacherId)
            && !string.IsNullOrWhiteSpace(slugClaim)
        )
        {
            return new UserContext(tenantId, teacherId, slugClaim);
        }

        return null;
    }
}
