namespace api.DTOs;

public record DashboardStatsCardDto(
    decimal TodayIncome,
    decimal MonthIncome,
    decimal OverdueAmount,
    int OverdueStudentsCount,
    int ActiveStudentsCount
);

public record MonthlyRevenueDto(string MonthKey, string MonthName, decimal TotalAmount);

public record DashboardTodaySessionDto(
    Guid? SessionId,
    Guid GroupId,
    string GroupDisplayName,
    string StartTime,
    string EndTime,
    string Status,
    int TotalStudentsCount
);

public record DashboardTeacherDataDto(
    DashboardStatsCardDto Stats,
    List<MonthlyRevenueDto> MonthlyRevenue,
    List<DashboardTodaySessionDto> TodaySessions
);
