using api.DTOs;

namespace api.Interfaces;

public interface IAuthService
{
    Task<ApiResponse<TenantAuthResponseDto>> RegisterTenantAsync(RegisterTenantDto dto);
    Task<ApiResponse<LoginResponseDto>> LoginAsync(string slug, LoginDto dto);
}
