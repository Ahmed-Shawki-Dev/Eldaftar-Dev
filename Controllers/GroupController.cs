using api.DTOs;
using api.Helpers;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/{slug}/teachers/{teacherId:guid}/groups")]
[Authorize(Roles = "Teacher,Staff")]
public class GroupController(IGroupService groupService) : BaseApiController
{
    // * 1.Create New Group
    [HttpPost]
    public async Task<IActionResult> CreateGroup(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromBody] CreateGroupDto dto
    )
    {
        // 1. Check Authorization & Ownership
        var userContext = User.GetUserContext();
        if (userContext == null || userContext.TeacherId != teacherId)
        {
            return ForbiddenRes("غير مصرح لك بالوصول لبيانات هذا المدرس");
        }

        // 2. Execute Business Logic
        var result = await groupService.CreateGroupAsync(
            userContext.TenantId,
            userContext.TeacherId,
            dto
        );
        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        // 3. Return 200 Created
        return Success(result.Data!, result.Message);
    }

    // * 2.Get All Groups
    [HttpGet]
    public async Task<IActionResult> GetGroups(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromQuery] GroupParamsDto parameters
    )
    {
        // 1. Check Authorization & Ownership
        var userContext = User.GetUserContext();
        if (userContext == null || userContext.TeacherId != teacherId)
        {
            return ForbiddenRes("غير مصرح لك بالوصول لبيانات هذا المدرس");
        }

        // 2. Execute Business Logic
        var result = await groupService.GetGroupsAsync(userContext.TenantId, teacherId, parameters);

        // 3. Return 200 OK
        return Success(result.Data!, result.Pagination!, result.Message);
    }
}
