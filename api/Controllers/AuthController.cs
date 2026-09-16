using System.Security.Principal;
using api.DTOs;
using api.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[Route("api")]
public class AuthController(IAuthService authService) : BaseApiController
{
    [HttpPost("auth/register-tenant")]
    public async Task<IActionResult> RegisterTenant([FromBody] RegisterTenantDto dto)
    {
        var result = await authService.RegisterTenantAsync(dto);
        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }
        return Success(result.Data!, result.Message);
    }

    [HttpPost("{slug}/auth/login")]
    public async Task<IActionResult> Login([FromRoute] string slug, [FromBody] LoginDto dto)
    {
        var result = await authService.LoginAsync(slug, dto);
        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }

        Response.Cookies.Append(
            "auth_token",
            result.Data!.Token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(7),
            }
        );

        return Success(result.Message);
    }

    [Authorize]
    [HttpPost("{slug}/auth/logout")]
    public IActionResult Logout([FromRoute] string slug)
    {
        Response.Cookies.Delete(
            "auth_token",
            new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
            }
        );

        return NoContent();
    }

    [Authorize]
    [HttpGet("{slug}/auth/me")]
    public IActionResult GetCurrentUser([FromRoute] string slug)
    {
        var userContext = User.GetUserContext();

        if (
            userContext is null
            || !string.Equals(userContext.TenantSlug, slug, StringComparison.OrdinalIgnoreCase)
        )
        {
            return ForbiddenRes("غير مصرح لك بالوصول لبيانات هذا السنتر.");
        }

        var dto = new CurrentUserDto(
            userContext.UserId,
            userContext.RoleClaim,
            userContext.TenantSlug,
            userContext.TenantId,
            userContext.TeacherId
        );

        return Success(dto, "تم التحقق من الجلسة بنجاح.");
    }
}
