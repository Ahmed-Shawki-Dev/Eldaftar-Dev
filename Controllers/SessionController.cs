namespace api.Controllers;

using api.DTOs;
using api.Helpers;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;

[Route("api/{slug}/teachers/{teacherId:guid}/sessions")]
public class SessionController(ISessionService sessionService) : BaseApiController
{
    // * 1. Get Daily Sessions Cards
    // GET: api/{slug}/teachers/{teacherId}/sessions/daily?date=2026-08-31
    [HttpGet("daily")]
    public async Task<IActionResult> GetDailySessions(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromQuery] DailySessionsParamsDto parameters
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

        var result = await sessionService.GetDailySessionsAsync(
            userContext.TenantId,
            teacherId,
            parameters
        );

        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        return Success(result.Data, result.Message);
    }

    // * 2. Start or Get-or-Create Session
    // POST: api/{slug}/teachers/{teacherId}/sessions/start
    [HttpPost("start")]
    public async Task<IActionResult> StartOrCreateSession(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromBody] StartSessionDto dto
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

        var result = await sessionService.StartOrCreateSessionAsync(
            userContext.TenantId,
            teacherId,
            dto
        );

        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        return Success(result.Data!, result.Message);
    }

    // * 3. Get Attendance Sheet View
    // GET: api/{slug}/teachers/{teacherId}/sessions/{sessionId}/sheet
    [HttpGet("{sessionId:guid}/sheet")]
    public async Task<IActionResult> GetSessionSheet(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromRoute] Guid sessionId
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

        var result = await sessionService.GetSessionSheetAsync(
            userContext.TenantId,
            teacherId,
            sessionId
        );

        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        return Success(result.Data, result.Message);
    }
}
