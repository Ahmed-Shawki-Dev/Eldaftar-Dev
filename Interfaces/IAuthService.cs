using api.DTOs;

namespace api.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<TenantAuthResponseDto>> RegisterTenantAsync(RegisterTenantDto dto);
}
