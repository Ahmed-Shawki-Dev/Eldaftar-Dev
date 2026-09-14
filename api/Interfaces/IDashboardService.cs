using api.DTOs;

namespace api.Interfaces;

public interface IDashboardService
{
    Task<ApiResponse<DashboardTeacherDataDto>> GetDashboardTeacherDataAsync(
        Guid tenantId,
        Guid teacherId
    );
}
