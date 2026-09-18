using System.Security.Claims;

public record UserContext(
    Guid UserId,
    Guid TenantId,
    Guid? RawTeacherId,
    string TenantSlug,
    string RoleClaim
)
{
    public Guid TeacherId => RawTeacherId.GetValueOrDefault();

    public Guid? NullableTeacherId => RawTeacherId;
}

public static class UserClaimsExtensions
{
    public static UserContext? GetUserContext(this ClaimsPrincipal user)
    {
        var userClaim = user.FindFirst("userId")?.Value;
        var tenantClaim = user.FindFirst("tenantId")?.Value;
        var teacherClaim = user.FindFirst("teacherId")?.Value;
        var slugClaim = user.FindFirst("tenantSlug")?.Value;
        var roleClaim = user.FindFirst("role")?.Value ?? user.FindFirst(ClaimTypes.Role)?.Value;

        Guid? teacherId = Guid.TryParse(teacherClaim, out var parsedTeacherId)
            ? parsedTeacherId
            : null;

        if (
            Guid.TryParse(userClaim, out var userId)
            && Guid.TryParse(tenantClaim, out var tenantId)
            && !string.IsNullOrWhiteSpace(slugClaim)
            && !string.IsNullOrWhiteSpace(roleClaim)
        )
        {
            return new UserContext(userId, tenantId, teacherId, slugClaim, roleClaim);
        }

        return null;
    }
}
