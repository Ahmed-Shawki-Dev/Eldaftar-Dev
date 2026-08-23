using System.Security.Claims;

namespace api.Helpers;

public static class CheckTeacherIdHelper
{
    public static bool TryGetTeacherId(ClaimsPrincipal user, out Guid teacherId)
    {
        var claimValue = user.FindFirst("teacherId")?.Value;

        if (!string.IsNullOrWhiteSpace(claimValue) && Guid.TryParse(claimValue, out teacherId))
        {
            return true;
        }

        teacherId = Guid.Empty;
        return false;
    }

    public static bool HasTeacherId(ClaimsPrincipal user)
    {
        return user.HasClaim(c => c.Type == "teacherId" && !string.IsNullOrWhiteSpace(c.Value));
    }
}
