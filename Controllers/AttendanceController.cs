namespace api.Controllers;

using api.DTOs;
using api.Helpers;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/{slug}/teachers/{teacherId:guid}/sessions/{sessionId:guid}/attendance")]
public class AttendanceController(IAttendanceService attendanceService) : BaseApiController
{
    // * Submit Bulk Attendance
    // POST: api/{slug}/teachers/{teacherId}/sessions/{sessionId}/attendance/bulk
    [HttpPost("bulk")]
    public async Task<IActionResult> BulkAttendance(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromRoute] Guid sessionId,
        [FromBody] BulkAttendanceDto dto
    )
    {
        var userContext = User.GetUserContext();
        if (
            userContext == null
            || userContext.TeacherId != teacherId
            || userContext.TenantSlug != slug
        )
        {
            return ForbiddenRes("غير مصرح لك بالوصول لبيانات هذا السنتر/المدرس.");
        }

        var result = await attendanceService.BulkAttendanceAsync(
            userContext.TenantId,
            teacherId,
            userContext.UserId,
            sessionId,
            dto
        );

        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        return Success(message: result.Message);
    }
}
