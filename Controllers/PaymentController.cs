using api.DTOs;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/{slug}/teachers/{teacherId:guid}/payments")]
[Authorize(Roles = "Teacher,Staff")]
public class PaymentController(IPaymentService paymentService) : BaseApiController
{
    [HttpPost("generate-monthly")]
    public async Task<IActionResult> GenerateMonthlyInvoicesAsync(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromQuery] string? monthKey
    )
    {
        var userContext = User.GetUserContext();
        if (
            userContext == null
            || userContext.TeacherId != teacherId
            || userContext.TenantSlug != slug
        )
        {
            return ForbiddenRes("غير مصرح لك بإجراء هذه العملية");
        }

        var result = await paymentService.GenerateMonthlyInvoicesAsync(
            userContext.TenantId,
            userContext.TeacherId,
            monthKey!
        );

        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        return NoContent();
    }

    [HttpGet("quick-search/{studentCode}")]
    public async Task<IActionResult> QuickSearchByStudentCode(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromRoute] string studentCode
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

        var result = await paymentService.GetStudentPendingInvoicesByCodeAsync(
            userContext.TenantId,
            teacherId,
            studentCode.Trim()
        );

        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        return Success(result.Data!, result.Message);
    }

    [HttpGet("groups/{groupId:guid}/sheet")]
    public async Task<IActionResult> GetPaymentSheet(
        [FromRoute] string slug,
        [FromRoute] Guid teacherId,
        [FromRoute] Guid groupId,
        [FromQuery] PaymentFilterDto filterDto
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

        var result = await paymentService.GetPaymentSheetAsync(
            userContext.TenantId,
            teacherId,
            groupId,
            filterDto
        );

        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        return Success(result.Data!, result.Message);
    }
}
