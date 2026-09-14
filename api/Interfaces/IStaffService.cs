using api.DTOs;

namespace api.Interfaces;

public interface IStaffService
{
    Task<ApiResponse<StaffDto>> CreateStaffAsync(Guid tenantId, Guid teacherId, CreateStaffDto dto);
    Task<ApiResponse<List<StaffDto>>> GetAllStaffAsync(Guid tenantId, Guid teacherId);
    Task<ApiResponse<StaffDto>> UpdateStaffAsync(
        Guid tenantId,
        Guid teacherId,
        Guid staffId,
        UpdateStaffDto dto
    );
    Task<ApiResponse<object>> DeleteStaffAsync(Guid tenantId, Guid teacherId, Guid staffId);
}
