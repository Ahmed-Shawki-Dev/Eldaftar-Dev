using api.Data;
using api.DTOs;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class DashboardService(ApplicationDBContext context) : IDashboardService
{
    public async Task<ApiResponse<DashboardTeacherDataDto>> GetDashboardTeacherDataAsync(
        Guid tenantId,
        Guid teacherId
    )
    {
        // 1. Get Dates To Make Queries
        var now = DateTimeOffset.Now;

        var startOfToday = new DateTimeOffset(
            now.Year,
            now.Month,
            now.Day,
            0,
            0,
            0,
            now.Offset
        ).ToUniversalTime();
        var startOfTomorrow = startOfToday.AddDays(1);

        var startOfMonth = new DateTimeOffset(
            now.Year,
            now.Month,
            1,
            0,
            0,
            0,
            now.Offset
        ).ToUniversalTime();
        var startOfNextMonth = startOfMonth.AddMonths(1);
        var sixMonthAgo = startOfMonth.AddMonths(-5);

        // 2. Get Total Daily Income
        var totalDailyIncome = await context
            .PaymentTransactions.IgnoreQueryFilters()
            .Where(pt =>
                pt.Invoice.Group.TeacherId == teacherId
                && pt.CreatedAt >= startOfToday
                && pt.CreatedAt < startOfTomorrow
            )
            .SumAsync(pt => pt.Amount);

        // 3. Get Total Monthly Income
        var totalMonthlyIncome = await context
            .PaymentTransactions.IgnoreQueryFilters()
            .Where(pt =>
                pt.Invoice.Group.TeacherId == teacherId
                && pt.CreatedAt >= startOfMonth
                && pt.CreatedAt < startOfNextMonth
            )
            .SumAsync(pt => pt.Amount);

        // 4. Get Total Student Counts
        var totalTeacherStudentsCount = await context
            .StudentGroups.Where(sg => sg.Group.TeacherId == teacherId)
            .Select(sg => sg.StudentId)
            .Distinct()
            .CountAsync();

        // 5. Get Total Overdue Amount And Total Overdue Students
        var unpaidInvoicesQuery = context.StudentInvoices.Where(si =>
            si.Group.TeacherId == teacherId && si.Status == InvoiceStatus.Unpaid
        );

        var totalOverdueAmount = await unpaidInvoicesQuery.SumAsync(si => si.TotalAmount);

        var overdueStudentsCount = await unpaidInvoicesQuery
            .Select(si => si.StudentId)
            .Distinct()
            .CountAsync();

        var statsCardDto = new DashboardStatsCardDto(
            TodayIncome: totalDailyIncome,
            MonthIncome: totalMonthlyIncome,
            OverdueAmount: totalOverdueAmount,
            OverdueStudentsCount: overdueStudentsCount,
            ActiveStudentsCount: totalTeacherStudentsCount
        );

        // 6. Get Total 6 Months Transaction To Chart
        var chartRevenueData = await context
            .PaymentTransactions.IgnoreQueryFilters()
            .Where(pt =>
                pt.Invoice.Group.TeacherId == teacherId
                && pt.CreatedAt >= sixMonthAgo
                && pt.CreatedAt < startOfNextMonth
            )
            .GroupBy(pt => new { pt.CreatedAt.Year, pt.CreatedAt.Month })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                Total = g.Sum(pt => pt.Amount),
            })
            .ToListAsync();

        var monthlyRevenue = new List<MonthlyRevenueDto>();

        for (int i = 5; i >= 0; i--)
        {
            var targetDate = now.AddMonths(-i);
            var year = targetDate.Year;
            var month = targetDate.Month;

            var record = chartRevenueData.FirstOrDefault(x => x.Year == year && x.Month == month);
            var total = record?.Total ?? 0m;

            var monthKey = $"{year}-{month:D2}";
            var monthName = targetDate.ToString(
                "MMMM",
                new System.Globalization.CultureInfo("ar-EG")
            );

            monthlyRevenue.Add(new MonthlyRevenueDto(monthKey, monthName, total));
        }

        // 7. Get Today's Sessions Based on Group Schedules
        var todayDayOfWeek = (DayOfWeekEnum)(int)now.DayOfWeek;

        var todaySessions = await context
            .GroupSchedules.Where(gs =>
                gs.Group.TeacherId == teacherId && gs.DayOfWeek == todayDayOfWeek
            )
            .OrderBy(gs => gs.StartTime)
            .Select(gs => new
            {
                Schedule = gs,
                TodaySession = gs.Group.Sessions.FirstOrDefault(s =>
                    s.SessionDate >= startOfToday && s.SessionDate < startOfTomorrow
                ),
            })
            .Select(x => new DashboardTodaySessionDto(
                x.TodaySession != null ? x.TodaySession.Id : null,
                x.Schedule.GroupId,
                $"{x.Schedule.Group.Grade} - {x.Schedule.Group.Name}",
                x.Schedule.StartTime,
                x.Schedule.EndTime,
                x.TodaySession != null
                    ? x.TodaySession.Status.ToString()
                    : SessionStatus.Scheduled.ToString(),
                x.Schedule.Group.StudentGroups.Count()
            ))
            .ToListAsync();

        var data = new DashboardTeacherDataDto(statsCardDto, monthlyRevenue, todaySessions);
        return ApiResponse<DashboardTeacherDataDto>.Ok(data, "تم جلب بيانات الداشبورد بنجاح");
    }
}
