using api.DTOs;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api/{slug}/payments")]
[Authorize(Roles = "Teacher,Staff")]
public class PaymentController(IPaymentService paymentService) : BaseApiController
{
    [HttpPost("generate-monthly")]
    public async Task<IActionResult> GenerateMonthlyInvoicesAsync(
        [FromRoute] string slug,
        [FromQuery] string? monthKey
    )
    {
        var userContext = User.GetUserContext();
        if (userContext == null || userContext.TenantSlug != slug)
        {
            return ForbiddenRes("غير مصرح لك بإجراء هذه العملية");
        }

        var result = await paymentService.GenerateMonthlyInvoicesAsync(
            userContext.TenantId,
            userContext.TeacherId,
            monthKey
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
        [FromRoute] string studentCode
    )
    {
        var userContext = User.GetUserContext();
        if (userContext == null || userContext.TenantSlug != slug)
        {
            return ForbiddenRes("غير مصرح لك بالوصول لبيانات هذا السنتر/المدرس.");
        }

        var result = await paymentService.GetStudentPendingInvoicesByCodeAsync(
            userContext.TenantId,
            userContext.TeacherId,
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
        [FromRoute] Guid groupId,
        [FromQuery] PaymentFilterDto filterDto
    )
    {
        var userContext = User.GetUserContext();
        if (userContext == null || userContext.TenantSlug != slug)
        {
            return ForbiddenRes("غير مصرح لك بالوصول لبيانات هذا السنتر/المدرس.");
        }

        var result = await paymentService.GetPaymentSheetAsync(
            userContext.TenantId,
            userContext.TeacherId,
            groupId,
            filterDto
        );

        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        return Success(result.Data!, result.Message);
    }

    [HttpPost("collect")]
    public async Task<IActionResult> CollectPaymentAsync(
        [FromRoute] string slug,
        [FromBody] CollectPaymentDto dto
    )
    {
        var userContext = User.GetUserContext();
        if (userContext == null || userContext.TenantSlug != slug)
        {
            return ForbiddenRes("غير مصرح لك بالوصول لبيانات هذا السنتر/المدرس.");
        }

        var result = await paymentService.CollectPaymentAsync(
            userContext.TenantId,
            userContext.TeacherId,
            userContext.UserId,
            dto
        );

        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        return Success(result.Message);
    }

    [HttpPost("cancel")]
    public async Task<IActionResult> CancelPaymentAsync(
        [FromRoute] string slug,
        [FromBody] CancelPaymentDto dto
    )
    {
        var userContext = User.GetUserContext();
        if (userContext == null || userContext.TenantSlug != slug)
        {
            return ForbiddenRes("غير مصرح لك بالوصول لبيانات هذا السنتر/المدرس.");
        }

        var result = await paymentService.CancelPaymentAsync(
            userContext.TenantId,
            userContext.TeacherId,
            userContext.UserId,
            userContext.RoleClaim,
            dto
        );

        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        return Success(result.Message);
    }
}
