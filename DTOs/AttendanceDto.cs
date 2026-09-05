using api.Models;

namespace api.DTOs;

public record SessionAttendanceSheetDto(
    Guid SessionId,
    string GroupName,
    string Grade,
    DateTimeOffset SessionDate,
    SessionStatus Status,
    PaymentType PaymentType,
    decimal DefaultGroupPrice,
    List<StudentAttendanceRowDto> Students
);

public record StudentAttendanceRowDto(
    Guid StudentId, 
    string StudentName,
    string StudentCode,
    AttendanceStatus AttendanceStatus,
    bool HasPaidSession,
    decimal? RequiredPrice,
    bool IsVisitorStudent
);
