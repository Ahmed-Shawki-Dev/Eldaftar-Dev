using api.DTOs;
using api.Helpers;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/{slug}/teachers/{teacherId:guid}/students")]
[Authorize(Roles = "Teacher,Staff")]
public class StudentController(IStudentService studentService) : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> CreateStudent(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromBody] CreateStudentDto dto
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
        var result = await studentService.CreateStudentAsync(
            userContext.TenantId,
            userContext.TeacherId,
            dto
        );

        if (!result.Success)
            return BadReq(result.Message, result.Errors);

        // 3. Return 200 Created
        return Success(result.Data!, result.Message);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllStudents(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromQuery] StudentParamsDto parameters
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
        var result = await studentService.GetAllStudentsAsync(
            userContext.TenantId,
            userContext.TeacherId,
            parameters
        );

        if (!result.Success)
            return BadReq(result.Message, result.Errors);

        // 3. Return 200 Created
        return Success(result.Data!, result.Pagination!, result.Message);
    }
}
