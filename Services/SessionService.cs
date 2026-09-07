namespace api.Services;

using api.Data;
using api.DTOs;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

public class SessionService(ApplicationDBContext context) : ISessionService
{
    // * 1. Get Daily Sessions Overview
    public async Task<ApiResponse<List<DailySessionCardDto>>> GetDailySessionsAsync(
        Guid tenantId,
        Guid teacherId,
        DailySessionsParamsDto parameters
    )
    {
        var targetDate = parameters.Date ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var targetDay = (DayOfWeekEnum)(int)targetDate.DayOfWeek;

        var startOfDay = new DateTimeOffset(
            targetDate.ToDateTime(TimeOnly.MinValue),
            TimeSpan.Zero
        );
        var endOfDay = new DateTimeOffset(targetDate.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero);

        // Fetch matching groups with their schedules for today and today's sessions
        var groups = await context
            .Groups.AsNoTracking()
            .Where(g =>
                g.TenantId == tenantId
                && g.TeacherId == teacherId
                && g.Schedules.Any(s => s.DayOfWeek == targetDay)
            )
            .Select(g => new
            {
                g.Id,
                g.Name,
                g.Grade,
                TodaySchedules = g.Schedules.Where(s => s.DayOfWeek == targetDay).ToList(),
                TodaySession = g
                    .Sessions.Where(s => s.SessionDate >= startOfDay && s.SessionDate <= endOfDay)
                    .Select(s => new { s.Id, s.Status })
                    .FirstOrDefault(),
            })
            .ToListAsync();

        // Project to Flat DTO List (One card per scheduled slot)
        var result = new List<DailySessionCardDto>();

        foreach (var group in groups)
        {
            foreach (var schedule in group.TodaySchedules)
            {
                result.Add(
                    new DailySessionCardDto(
                        SessionId: group.TodaySession?.Id,
                        GroupId: group.Id,
                        GroupName: group.Name,
                        Grade: group.Grade,
                        StartTime: schedule.StartTime,
                        EndTime: schedule.EndTime,
                        TargetDate: startOfDay,
                        Status: group.TodaySession?.Status ?? SessionStatus.Scheduled
                    )
                );
            }
        }

        return ApiResponse<List<DailySessionCardDto>>.Ok(result, "تم جلب حصص اليوم بنجاح.");
    }

    // * 2. Start / Get-or-Create Session
    public async Task<ApiResponse<object>> StartOrCreateSessionAsync(
        Guid tenantId,
        Guid teacherId,
        StartSessionDto dto
    )
    {
        // 1. Validate Group Ownership
        var groupExists = await context.Groups.AnyAsync(g =>
            g.Id == dto.GroupId && g.TenantId == tenantId && g.TeacherId == teacherId
        );

        if (!groupExists)
        {
            return ApiResponse<object>.Fail("المجموعة غير موجودة أو غير تابعة لهذا المدرس.");
        }

        // 2. Check if a session already exists for this date
        var targetDate = DateOnly.FromDateTime(dto.SessionDate.UtcDateTime);
        var startOfDay = new DateTimeOffset(
            targetDate.ToDateTime(TimeOnly.MinValue),
            TimeSpan.Zero
        );
        var endOfDay = new DateTimeOffset(targetDate.ToDateTime(TimeOnly.MaxValue), TimeSpan.Zero);

        var existingSession = await context.Sessions.FirstOrDefaultAsync(s =>
            s.GroupId == dto.GroupId && s.SessionDate >= startOfDay && s.SessionDate <= endOfDay
        );

        if (existingSession != null)
        {
            return ApiResponse<object>.Ok(existingSession.Id, "تم العثور على الحصة مسبقاً.");
        }

        // 3. Create new Session
        var newSession = new Session
        {
            GroupId = dto.GroupId,
            SessionDate = dto.SessionDate,
            Status = SessionStatus.Scheduled,
        };

        await context.Sessions.AddAsync(newSession);
        await context.SaveChangesAsync();

        return ApiResponse<object>.Ok(newSession.Id, "تم بدء الحصة بنجاح.");
    }

    public async Task<ApiResponse<SessionAttendanceSheetDto>> GetSessionSheetAsync(
        Guid tenantId,
        Guid teacherId,
        Guid sessionId
    )
    {
        // 1. Get Target Session
        var session = await context
            .Sessions.Include(s => s.Group)
            .AsNoTracking()
            .FirstOrDefaultAsync(s =>
                s.Id == sessionId && s.Group.TenantId == tenantId && s.Group.TeacherId == teacherId
            );

        if (session == null)
        {
            return ApiResponse<SessionAttendanceSheetDto>.Fail(
                "الحصة غير موجودة أو غير تابعة لهذا المدرس."
            );
        }

        // 2. Get All Active Enrolled Students in this Group
        var studentsEnrolledToGroup = await context
            .StudentGroups.Include(sg => sg.Student)
            .AsNoTracking()
            .Where(sg => sg.GroupId == session.GroupId)
            .ToListAsync();

        // 3. Get Previous Attendance Records mapped to Dictionary
        var attendanceMap = await context
            .Attendances.Include(a => a.Student)
            .AsNoTracking()
            .Where(a => a.SessionId == sessionId)
            .ToDictionaryAsync(a => a.StudentId);

        // 4. If Group PaymentType is PerSession, fetch paid student IDs
        var paidStudentIds = new HashSet<Guid>();
        if (session.Group.PaymentType == PaymentType.PerSession)
        {
            paidStudentIds = (
                await context
                    .StudentInvoices.AsNoTracking()
                    .Where(i =>
                        i.SessionId == sessionId
                        && i.Type == InvoiceType.PerSession
                        && i.Status == InvoiceStatus.Paid
                    )
                    .Select(i => i.StudentId)
                    .ToListAsync()
            ).ToHashSet();
        }

        // 5. Merge Enrolled Students
        var enrolledStudentIds = new HashSet<Guid>();
        var studentRows = new List<StudentAttendanceRowDto>();

        foreach (var enrollment in studentsEnrolledToGroup)
        {
            enrolledStudentIds.Add(enrollment.StudentId);
            var hasAttendance = attendanceMap.TryGetValue(enrollment.StudentId, out var attendance);

            studentRows.Add(
                new StudentAttendanceRowDto(
                    StudentId: enrollment.StudentId,
                    StudentName: enrollment.Student.Name,
                    StudentCode: enrollment.Student.StudentCode,
                    AttendanceStatus: hasAttendance ? attendance!.Status : AttendanceStatus.Absent,
                    HasPaidSession: paidStudentIds.Contains(enrollment.StudentId),
                    RequiredPrice: session.Group.PaymentType == PaymentType.PerSession
                        ? (enrollment.CustomPrice ?? session.Group.Price)
                        : null,
                    IsVisitorStudent: false
                )
            );
        }

        // 6. Append Visitors
        foreach (var (studentId, attendance) in attendanceMap)
        {
            if (!enrolledStudentIds.Contains(studentId))
            {
                studentRows.Add(
                    new StudentAttendanceRowDto(
                        StudentId: attendance.StudentId,
                        StudentName: attendance.Student.Name,
                        StudentCode: attendance.Student.StudentCode,
                        AttendanceStatus: attendance.Status,
                        HasPaidSession: paidStudentIds.Contains(attendance.StudentId),
                        RequiredPrice: session.Group.PaymentType == PaymentType.PerSession
                            ? session.Group.Price
                            : null,
                        IsVisitorStudent: true
                    )
                );
            }
        }

        // 7. Assemble Full Sheet DTO
        var sheetDto = new SessionAttendanceSheetDto(
            SessionId: session.Id,
            GroupName: session.Group.Name,
            Grade: session.Group.Grade,
            SessionDate: session.SessionDate,
            Status: session.Status,
            PaymentType: session.Group.PaymentType,
            DefaultGroupPrice: session.Group.Price,
            Students: studentRows
        );

        return ApiResponse<SessionAttendanceSheetDto>.Ok(sheetDto, "تم جلب شيت الحضور بنجاح.");
    }
}
