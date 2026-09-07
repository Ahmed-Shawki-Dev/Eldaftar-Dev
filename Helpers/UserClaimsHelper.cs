using System.Security.Claims;

public record UserContext(Guid UserId, Guid TenantId, Guid TeacherId, string TenantSlug);

public static class UserClaimsExtensions
{
    public static UserContext? GetUserContext(this ClaimsPrincipal user)
    {
        var userClaim = user.FindFirst("userId")?.Value;
        var tenantClaim = user.FindFirst("tenantId")?.Value;
        var teacherClaim = user.FindFirst("teacherId")?.Value;
        var slugClaim = user.FindFirst("tenantSlug")?.Value;

        if (
            Guid.TryParse(userClaim, out var userId)
            && Guid.TryParse(tenantClaim, out var tenantId)
            && Guid.TryParse(teacherClaim, out var teacherId)
            && !string.IsNullOrWhiteSpace(slugClaim)
        )
        {
            return new UserContext(userId, tenantId, teacherId, slugClaim);
        }

        return null;
    }
}
