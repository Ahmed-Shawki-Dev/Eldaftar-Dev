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
}
