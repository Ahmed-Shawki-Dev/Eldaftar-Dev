using api.DTOs;
using api.Helpers;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/{slug}/staff")]
[Authorize(Roles = "Teacher")]
public class StaffController(IStaffService staffService) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateStaff(
        [FromRoute] string slug,
        [FromBody] CreateStaffDto dto
    )
    {
        var userContext = User.GetUserContext();
        if (
            userContext == null
            || userContext.TenantSlug != slug
        )
        {
            return ForbiddenRes("غير مصرح لك بإضافة سكرتارية لهذا المدرس");
        }

        var result = await staffService.CreateStaffAsync(
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

    [HttpGet]
    public async Task<IActionResult> GetAllStaff(
        [FromRoute] string slug
    )
    {
        var userContext = User.GetUserContext();
        if (
            userContext == null
            || userContext.TenantSlug != slug
        )
        {
            return ForbiddenRes("غير مصرح لك بعرض سكرتارية هذا المدرس");
        }

        var result = await staffService.GetAllStaffAsync(
            userContext.TenantId,
            userContext.TeacherId
        );

        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        return Success(result.Data!, result.Message);
    }

    [HttpPut("{staffId:guid}")]
    public async Task<IActionResult> UpdateStaff(
        [FromRoute] string slug,
        [FromRoute] Guid staffId,
        [FromBody] UpdateStaffDto dto
    )
    {
        var userContext = User.GetUserContext();
        if (
            userContext == null
            || userContext.TenantSlug != slug
        )
        {
            return ForbiddenRes("غير مصرح لك بتعديل بيانات هذه السكرتيرة");
        }

        var result = await staffService.UpdateStaffAsync(
            userContext.TenantId,
            userContext.TeacherId,
            staffId,
            dto
        );

        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        return Success(result.Data!, result.Message);
    }

    [HttpDelete("{staffId:guid}")]
    public async Task<IActionResult> DeleteStaff(
        [FromRoute] string slug,
        [FromRoute] Guid staffId
    )
    {
        var userContext = User.GetUserContext();
        if (
            userContext == null
            || userContext.TenantSlug != slug
        )
        {
            return ForbiddenRes("غير مصرح لك بحذف هذه السكرتيرة");
        }

        var result = await staffService.DeleteStaffAsync(
            userContext.TenantId,
            userContext.TeacherId,
            staffId
        );

        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        return Success(result.Message);
    }
}
