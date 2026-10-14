namespace api.Controllers;

using api.DTOs;
using api.Filters;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/{slug}/sessions/{sessionId:guid}/attendance")]
[Authorize]
[ServiceFilter(typeof(CheckTeacherTenantFilter))]
public class AttendanceController(IAttendanceService attendanceService) : BaseApiController
{
    // * Submit Bulk Attendance
    [HttpPost("bulk")]
    public async Task<IActionResult> BulkAttendance(
        [FromRoute] string slug,
        [FromRoute] Guid sessionId,
        [FromBody] BulkAttendanceDto dto
    )
    {
        var userContext = (UserContext)HttpContext.Items["UserContext"]!;

        var result = await attendanceService.BulkAttendanceAsync(
            userContext.TenantId,
            userContext.TeacherId,
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

    // * Add Visitor Student to Session
    [HttpPost("visitors")]
    public async Task<IActionResult> AddVisitorStudent(
        [FromRoute] string slug,
        [FromRoute] Guid sessionId,
        [FromBody] AddVisitorStudentDto dto
    )
    {
        var userContext = (UserContext)HttpContext.Items["UserContext"]!;

        var result = await attendanceService.AddVisitorStudentToSessionAsync(
            userContext.TenantId,
            userContext.TeacherId,
            userContext.UserId,
            sessionId,
            dto
        );

        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        return Success(result.Data!, result.Message);
    }
}
