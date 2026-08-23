using api.Models;

namespace api.DTOs;

public record CreateScheduleDto(DayOfWeekEnum DayOfWeek, string StartTime, string EndTime);

public record CreateGroupDto(
    string Grade,
    string Name,
    decimal Price,
    PaymentType PaymentType,
    Guid? AcademicTermId,
    List<CreateScheduleDto> Schedules
);

public record UpdateScheduleDto(
    Guid? Id,
    DayOfWeekEnum DayOfWeek,
    string StartTime,
    string EndTime
);

public record UpdateGroupDto(
    string Name,
    string Grade,
    decimal Price,
    PaymentType PaymentType,
    Guid? AcademicTermId,
    bool IsActive,
    List<UpdateScheduleDto> Schedules
);

public record GroupSummaryDto(
    Guid Id,
    string Grade,
    string Name,
    decimal Price,
    int ActiveStudentsCount,
    PaymentType PaymentType,
    List<CreateScheduleDto> Schedules
);

public record GroupStudentDto(
    Guid StudentId,
    string StudentCode,
    string StudentName,
    string ParentPhone,
    string? Phone,
    EnrollmentStatus Status,
    decimal? CustomPrice
);

public record GroupDetailsDto(
    Guid Id,
    string Grade,
    string Name,
    decimal Price,
    PaymentType PaymentType,
    Guid? AcademicTermId,
    List<CreateScheduleDto> Schedules,
    List<GroupStudentDto> Students
);

public record GroupFilterParams(
    int PageNumber = 1,
    int PageSize = 10,
    string? Grade = null,
    string? SearchTerm = null
);
