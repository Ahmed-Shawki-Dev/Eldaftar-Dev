using api.DTOs;
using api.Helpers;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/{slug}/teachers/{teacherId:guid}/exams")]
[Authorize(Roles = "Teacher,Staff")]
public class ExamController(IExamService examService) : BaseApiController
{
    // * 1.Create New Exam
    [HttpPost]
    public async Task<IActionResult> CreateExam(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromBody] CreateExamDto dto
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
        var result = await examService.CreateExamAsync(
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

    // * 2. Get All Exams
    [HttpGet]
    public async Task<IActionResult> GetAllExams(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromQuery] ExamParamsDto parameters
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
        var result = await examService.GetAllExamsAsync(
            userContext.TenantId,
            userContext.TeacherId,
            parameters
        );
        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        // 3. Return 200 Created
        return Success(result.Data!, result.Pagination!, result.Message);
    }

    // * 3. Get Exam By Id
    [HttpGet("{examId:guid}")]
    public async Task<IActionResult> GetExamById(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromRoute] Guid examId
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
        var result = await examService.GetExamByIdAsync(
            userContext.TenantId,
            userContext.TeacherId,
            examId
        );
        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        // 3. Return 200 Created
        return Success(result.Data!, result.Message);
    }

    // * 4. Update Exam
    [HttpPut("{examId:guid}")]
    public async Task<IActionResult> UpdateExam(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromRoute] Guid examId,
        [FromBody] UpdateExamDto dto
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
        var result = await examService.UpdateExamAsync(
            userContext.TenantId,
            userContext.TeacherId,
            examId,
            dto
        );
        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        // 3. Return 200 Created
        return Success(result.Data!, result.Message);
    }

    // * 5. Delete Exam
    [HttpDelete("{examId:guid}")]
    public async Task<IActionResult> DeleteExam(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromRoute] Guid examId
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
        var result = await examService.DeleteExamAsync(
            userContext.TenantId,
            userContext.TeacherId,
            examId
        );
        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        // 3. Return 200 Created
        return Success(result.Message);
    }

    // * 6. Get Exam Sheet
    [HttpGet("{examId:guid}/sheet")]
    public async Task<IActionResult> GetExamSheet(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromRoute] Guid examId
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
        var result = await examService.GetExamSheetAsync(
            userContext.TenantId,
            userContext.TeacherId,
            examId
        );
        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        // 3. Return 200 Created
        return Success(result.Data!, result.Message);
    }

    // * 7. Put Exam Sheet
    [HttpPost("{examId:guid}/sheet")]
    public async Task<IActionResult> UpdateExamSheet(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromRoute] Guid examId,
        [FromBody] UpdateExamSheetDto dto
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
        var result = await examService.SaveBulkExamSheetAsync(
            userContext.TenantId,
            userContext.TeacherId,
            examId,
            dto
        );
        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        // 3. Return 200 Created
        return Success(result.Message);
    }
}
