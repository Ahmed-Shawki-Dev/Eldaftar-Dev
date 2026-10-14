using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace api.Filters;

public class CheckTeacherTenantFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    )
    {
        // 1. Validate Route Parameter
        if (
            !context.ActionArguments.TryGetValue("slug", out var slugObj)
            || slugObj is not string tenantSlugRoute
        )
        {
            context.Result = new ObjectResult(ApiResponse<object>.Fail("مسار غير صالح."))
            {
                StatusCode = StatusCodes.Status400BadRequest,
            };

            return;
        }

        // 2. Get User Context
        var userContext = context.HttpContext.User.GetUserContext();
        var tenantSlugContext = userContext?.TenantSlug;

        // 3. Ownership & Tenant Check
        if (userContext == null || tenantSlugContext != tenantSlugRoute)
        {
            context.Result = new ObjectResult(
                ApiResponse<object>.Fail("غير مصرح لك بالوصول لبيانات هذا السنتر/المدرس.")
            )
            {
                StatusCode = StatusCodes.Status403Forbidden,
            };

            return;
        }

        // 4. Pass Data & Continue
        context.HttpContext.Items["UserContext"] = userContext;

        await next();
    }
}
