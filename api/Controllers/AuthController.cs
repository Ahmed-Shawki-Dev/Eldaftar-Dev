using api.DTOs;
using api.Interfaces;
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
        return Success(result.Data!, result.Message);
    }
}
