using api.DTOs;
using api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

public class AuthController(IAuthService authService) : BaseApiController
{
    [HttpPost("register-tenant")]
    public async Task<IActionResult> RegisterTenant([FromBody] RegisterTenantDto dto)
    {
        var result = await authService.RegisterTenantAsync(dto);
        if (!result.Success)
        {
            return BadReq(result.Message, result.Errors);
        }
        return Success(result.Data!, result.Message);
    }

    [HttpPost("/api/{slug}/Auth/login")]
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
