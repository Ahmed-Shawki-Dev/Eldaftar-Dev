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
        if (
            userContext == null
            || userContext.TeacherId != teacherId
            || userContext.TenantSlug != slug
        )
        {
            return ForbiddenRes("غير مصرح لك بالوصول لبيانات هذا السنتر/المدرس");
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
        if (
            userContext == null
            || userContext.TeacherId != teacherId
            || userContext.TenantSlug != slug
        )
        {
            return ForbiddenRes("غير مصرح لك بالوصول لبيانات هذا السنتر/المدرس");
        }

        // 2. Execute Business Logic
        var result = await groupService.GetGroupsAsync(userContext.TenantId, teacherId, parameters);

        // 3. Return 200 OK
        return Success(result.Data!, result.Pagination!, result.Message);
    }

    // * 3.Get Group By Id
    [HttpGet("{groupId:guid}")]
    public async Task<IActionResult> GetGroupById(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromRoute] Guid groupId
    )
    {
        // 1. Check Authorization & Ownership
        var userContext = User.GetUserContext();
        if (
            userContext == null
            || userContext.TeacherId != teacherId
            || userContext.TenantSlug != slug
        )
        {
            return ForbiddenRes("غير مصرح لك بالوصول لبيانات هذا السنتر/المدرس");
        }

        // 2. Execute Business Logic
        var result = await groupService.GetGroupByIdAsync(
            userContext.TenantId,
            userContext.TeacherId,
            groupId
        );
        if (!result.Success)
        {
            return NotFoundRes(result.Message);
        }

        // 3. Return 200 OK
        return Success(result.Data!, result.Message);
    }

    // * 4.Update Existing Group
    [HttpPut("{groupId:guid}")]
    public async Task<IActionResult> UpdateGroup(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromRoute] Guid groupId,
        [FromBody] UpdateGroupDto dto
    )
    {
        // 1. Check Authorization & Ownership
        var userContext = User.GetUserContext();
        if (
            userContext == null
            || userContext.TeacherId != teacherId
            || userContext.TenantSlug != slug
        )
        {
            return ForbiddenRes("غير مصرح لك بالوصول لبيانات هذا السنتر/المدرس");
        }

        // 2. Execute Business Logic
        var result = await groupService.UpdateGroupAsync(
            userContext.TenantId,
            teacherId,
            groupId,
            dto
        );

        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        // 3. Return 200 OK
        return Success(result.Data!, result.Message);
    }

    // * 5.Delete Group
    [HttpDelete("{groupId:guid}")]
    public async Task<IActionResult> DeleteGroup(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromRoute] Guid groupId
    )
    {
        // 1. Check Authorization & Ownership
        var userContext = User.GetUserContext();
        if (
            userContext == null
            || userContext.TeacherId != teacherId
            || userContext.TenantSlug != slug
        )
        {
            return ForbiddenRes("غير مصرح لك بالوصول لبيانات هذا السنتر/المدرس");
        }

        // 2. Execute Business Logic
        var result = await groupService.SoftDeleteGroupAsync(
            userContext.TenantId,
            userContext.TeacherId,
            groupId
        );
        if (!result.Success)
        {
            return NotFoundRes(result.Message);
        }

        // 3. Return 200 OK
        return Success(result.Message);
    }
}
