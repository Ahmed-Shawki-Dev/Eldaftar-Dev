namespace api.Interfaces;

using api.DTOs;

public interface ISessionService
{
    Task<ApiResponse<List<DailySessionCardDto>>> GetDailySessionsAsync(
        Guid tenantId,
        Guid teacherId,
        DailySessionsParamsDto parameters
    );

    Task<ApiResponse<object>> StartOrCreateSessionAsync(
        Guid tenantId,
        Guid teacherId,
        StartSessionDto dto
    );

    Task<ApiResponse<SessionAttendanceSheetDto>> GetSessionSheetAsync(
        Guid tenantId,
        Guid teacherId,
        Guid sessionId
    );
}
