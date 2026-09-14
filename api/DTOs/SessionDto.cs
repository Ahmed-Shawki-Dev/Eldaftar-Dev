using api.Models;

namespace api.DTOs;

public record DailySessionCardDto(
    Guid? SessionId,
    Guid GroupId,
    string GroupName,
    string Grade,
    string StartTime,
    string EndTime,
    DateTimeOffset TargetDate,
    SessionStatus Status
);

public record StartSessionDto(Guid GroupId, DateTimeOffset SessionDate);

public record DailySessionsParamsDto(DateOnly? Date, Guid? GroupId);
