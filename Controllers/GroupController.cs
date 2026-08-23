using api.DTOs;
using api.Helpers;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/{slug}/teachers/{teacherId:guid}/groups")]
// [Authorize(Roles = "Teacher")]
public class GroupController(IGroupService groupService) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateGroup(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromBody] CreateGroupDto dto
    )
    {
        // 1. Check Authorization & Ownership
        if (
            !CheckTeacherIdHelper.TryGetTeacherId(User, out var tokenTeacherId)
            || tokenTeacherId != teacherId
        )
        {
            return ForbiddenRes("غير مصرح لك بالوصول لبيانات هذا المدرس");
        }

        // 2. Execute Business Logic
        var result = await groupService.CreateGroupAsync(teacherId, dto);

        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        // 3. Return 201 Created
        return Create(
            nameof(GetGroupById),
            new
            {
                slug,
                teacherId,
                id = result.Data!.Id,
            },
            result.Data,
            result.Message
        );
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGroupById(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromRoute] Guid id
    )
    {
        // 1. Check Authorization & Ownership
        if (
            !CheckTeacherIdHelper.TryGetTeacherId(User, out var tokenTeacherId)
            || tokenTeacherId != teacherId
        )
        {
            return ForbiddenRes("غير مصرح لك بالوصول لبيانات هذا المدرس");
        }

        // 2. Execute Business Logic
        var result = await groupService.GetGroupByIdAsync(teacherId, id);

        if (!result.Success)
        {
            return NotFoundRes(result.Message, result.Errors);
        }

        // 3. Return Success via BaseApiController Helper
        return Success(result.Data!, "تم إرجاع بيانات المجموعة بنجاح");
    }
}
