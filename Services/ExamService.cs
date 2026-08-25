using api.Data;
using api.DTOs;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class ExamService(ApplicationDBContext context) : IExamService
{
    public async Task<ApiResponse<ExamDto>> CreateExamAsync(
        Guid tenantId,
        Guid teacherId,
        CreateExamDto dto
    )
    {
        Group? group = null;

        // 1. Validate GroupId and Grade match if GroupId is provided
        if (dto.GroupId.HasValue)
        {
            group = await context.Groups.FirstOrDefaultAsync(g =>
                g.Id == dto.GroupId.Value && g.TeacherId == teacherId
            );

            if (group == null)
                return ApiResponse<ExamDto>.Fail("المجموعة غير موجودة أو لا تتبع هذا المدرس.");

            if (group.Grade != dto.Grade)
                return ApiResponse<ExamDto>.Fail(
                    "المجموعة المختارة لا تطابق المرحلة الدراسية للامتحان."
                );
        }

        // 2. Instantiate and persist the Exam entity
        var exam = new Exam
        {
            Title = dto.Title.Trim(),
            Grade = dto.Grade.Trim(),
            MaxScore = dto.MaxScore,
            Date = dto.Date ?? DateTimeOffset.UtcNow,
            GroupId = dto.GroupId,
            Group = group,
            TeacherId = teacherId,
        };

        await context.Exams.AddAsync(exam);
        await context.SaveChangesAsync();

        // 3. Map to DTO and return
        return ApiResponse<ExamDto>.Ok(exam.ToExamDto(), "تم إنشاء الامتحان بنجاح.");
    }

    public async Task<ApiResponse<List<ExamDto>>> GetAllExamsAsync(
        Guid tenantId,
        Guid teacherId,
        ExamParamsDto parameters
    )
    {
        var query = context
            .Exams.AsNoTracking()
            .Include(e => e.Group)
            .Where(e => e.TeacherId == teacherId && e.Teacher.TenantId == tenantId);

        // 2. Search Filter With Grade Or Name
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var search = parameters.SearchTerm.Trim().ToLower();
            query = query.Where(e =>
                e.Title.ToLower().Contains(search) || e.Grade.ToLower().Contains(search)
            );
        }

        // 3. Grade Filter
        if (!string.IsNullOrWhiteSpace(parameters.Grade))
        {
            query = query.Where(e => e.Grade == parameters.Grade);
        }

        // 4. GroupId Filter
        if (parameters.GroupId.HasValue)
        {
            query = query.Where(e => e.GroupId == parameters.GroupId.Value);
        }

        // 4. Count Total Records
        var totalRecords = await query.CountAsync();

        // 5. Apply Sorting & Pagination
        var exams = await query
            .OrderBy(g => g.Grade)
            .ThenBy(g => g.GroupId)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        // 6. Calculate Pagination Metadata
        var pagination = PaginationMetadata.Create(
            parameters.PageNumber,
            parameters.PageSize,
            totalRecords
        );
        var examDtos = exams.ToExamDtoList();

        return ApiResponse<List<ExamDto>>.OkPaged(examDtos, pagination, "تم جلب الامتحانات بنجاح.");
    }

    public async Task<ApiResponse<ExamDto>> GetExamByIdAsync(
        Guid tenantId,
        Guid teacherId,
        Guid examId
    )
    {
        var exam = await context
            .Exams.AsNoTracking()
            .Include(e => e.Group)
            .FirstOrDefaultAsync(e => e.Id == examId && e.TeacherId == teacherId);

        if (exam == null)
        {
            return ApiResponse<ExamDto>.Fail("الامتحان غير موجود.");
        }

        return ApiResponse<ExamDto>.Ok(exam.ToExamDto(), "تم جلب بيانات الامتحان بنجاح.");
    }

    public async Task<ApiResponse<ExamDto>> UpdateExamAsync(
        Guid tenantId,
        Guid teacherId,
        Guid examId,
        UpdateExamDto dto
    )
    {
        var exam = await context
            .Exams.Include(e => e.Group)
            .FirstOrDefaultAsync(e => e.Id == examId && e.TeacherId == teacherId);

        if (exam == null)
        {
            return ApiResponse<ExamDto>.Fail("الإمتحان غير موجود");
        }

        Group? group = null;

        if (dto.GroupId.HasValue)
        {
            group = await context.Groups.FirstOrDefaultAsync(g =>
                g.Id == dto.GroupId.Value && g.TeacherId == teacherId
            );

            if (group == null)
                return ApiResponse<ExamDto>.Fail("المجموعة غير موجودة أو لا تتبع هذا المدرس.");

            if (group.Grade != dto.Grade)
                return ApiResponse<ExamDto>.Fail(
                    "المجموعة المختارة لا تطابق المرحلة الدراسية للامتحان."
                );
        }

        exam.Title = dto.Title.Trim();
        exam.Grade = dto.Grade.Trim();
        exam.MaxScore = dto.MaxScore;
        exam.Date = dto.Date;
        exam.GroupId = dto.GroupId;
        exam.Group = group;

        await context.SaveChangesAsync();

        var examDto = exam.ToExamDto();

        return ApiResponse<ExamDto>.Ok(examDto, "تم تحديث بيانات الامتحان بنجاح");
    }

    public async Task<ApiResponse<object>> DeleteExamAsync(
        Guid tenantId,
        Guid teacherId,
        Guid examId
    )
    {
        var exam = await context.Exams.FirstOrDefaultAsync(e =>
            e.Id == examId && e.TeacherId == teacherId
        );

        if (exam == null)
        {
            return ApiResponse<object>.Fail("الامتحان غير موجود.");
        }

        context.Exams.Remove(exam);
        await context.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { }, "تم حذف الامتحان بنجاح.");
    }
}
