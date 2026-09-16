using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/{slug}/dashboard")]
[Authorize(Roles = "Teacher")]
public class DashboardController(IDashboardService dashboardService) : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> GetTeacherDashboardDataAsync([FromRoute] string slug)
    {
        // 1. Check Authorization & Ownership
        var userContext = User.GetUserContext();
        if (userContext == null || userContext.TenantSlug != slug)
        {
            return ForbiddenRes("غير مصرح لك بالوصول لبيانات هذا السنتر/المدرس");
        }

        // 2. Execute Business Logic
        var result = await dashboardService.GetDashboardTeacherDataAsync(
            userContext.TenantId,
            userContext.TeacherId
        );

        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        // 3. Return 200 OK
        return Success(result.Data!, result.Message);
    }
}
