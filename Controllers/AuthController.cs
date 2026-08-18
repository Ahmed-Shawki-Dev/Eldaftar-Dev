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
}
