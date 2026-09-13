using System.Security.Claims;

public record UserContext(
    Guid UserId,
    Guid TenantId,
    Guid TeacherId,
    string TenantSlug,
    string RoleClaim
);

public static class UserClaimsExtensions
{
    public static UserContext? GetUserContext(this ClaimsPrincipal user)
    {
        var userClaim = user.FindFirst("userId")?.Value;
        var tenantClaim = user.FindFirst("tenantId")?.Value;
        var teacherClaim = user.FindFirst("teacherId")?.Value;
        var slugClaim = user.FindFirst("tenantSlug")?.Value;
        var roleClaim = user.FindFirst("role")?.Value ?? user.FindFirst(ClaimTypes.Role)?.Value;

        if (
            Guid.TryParse(userClaim, out var userId)
            && Guid.TryParse(tenantClaim, out var tenantId)
            && Guid.TryParse(teacherClaim, out var teacherId)
            && !string.IsNullOrWhiteSpace(slugClaim)
            && !string.IsNullOrWhiteSpace(roleClaim)
        )
        {
            return new UserContext(userId, tenantId, teacherId, slugClaim, roleClaim);
        }

        return null;
    }
}
