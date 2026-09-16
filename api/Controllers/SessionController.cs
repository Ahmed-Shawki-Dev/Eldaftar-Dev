namespace api.Controllers;

using api.DTOs;
using api.Helpers;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/{slug}/sessions")]
[Authorize(Roles = "Teacher,Staff")]
public class SessionController(ISessionService sessionService) : BaseApiController
{
    // * 1. Get Daily Sessions Cards
    [HttpGet("daily")]
    public async Task<IActionResult> GetDailySessions(
        [FromRoute] string slug,
        [FromQuery] DailySessionsParamsDto parameters
    )
    {
        var userContext = User.GetUserContext();
        if (
            userContext == null
            || userContext.TenantSlug != slug
        )
        {
            return ForbiddenRes("غير مصرح لك بالوصول لبيانات هذا السنتر/المدرس.");
        }

        var result = await sessionService.GetDailySessionsAsync(
            userContext.TenantId,
            userContext.TeacherId,
            parameters
        );

        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        return Success(result.Data!, result.Message);
    }

    // * 2. Start or Get-or-Create Session
    [HttpPost("start")]
    public async Task<IActionResult> StartOrCreateSession(
        [FromRoute] string slug,
        [FromBody] StartSessionDto dto
    )
    {
        var userContext = User.GetUserContext();
        if (
            userContext == null
            || userContext.TenantSlug != slug
        )
        {
            return ForbiddenRes("غير مصرح لك بالوصول لبيانات هذا السنتر/المدرس.");
        }

        var result = await sessionService.StartOrCreateSessionAsync(
            userContext.TenantId,
            userContext.TeacherId,
            dto
        );

        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        return Success(result.Data!, result.Message);
    }

    // * 3. Get Attendance Sheet View
    [HttpGet("{sessionId:guid}/sheet")]
    public async Task<IActionResult> GetSessionSheet(
        [FromRoute] string slug,
        [FromRoute] Guid sessionId
    )
    {
        var userContext = User.GetUserContext();
        if (
            userContext == null
            || userContext.TenantSlug != slug
        )
        {
            return ForbiddenRes("غير مصرح لك بالوصول لبيانات هذا السنتر/المدرس.");
        }

        var result = await sessionService.GetSessionSheetAsync(
            userContext.TenantId,
            userContext.TeacherId,
            sessionId
        );

        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        return Success(result.Data!, result.Message);
    }
}
