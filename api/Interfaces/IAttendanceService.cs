using api.DTOs;

namespace api.Interfaces;

public interface IAttendanceService
{
    Task<ApiResponse<object>> BulkAttendanceAsync(
        Guid tenantId,
        Guid teacherId,
        Guid userId,
        Guid SessionId,
        BulkAttendanceDto dto
    );

    Task<ApiResponse<StudentAttendanceRowDto>> AddVisitorStudentToSessionAsync(
        Guid tenantId,
        Guid teacherId,
        Guid userId,
        Guid SessionId,
        AddVisitorStudentDto dto
    );
}
