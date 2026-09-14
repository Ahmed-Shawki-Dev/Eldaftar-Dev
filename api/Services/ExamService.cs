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

    // Exam Sheet
    public async Task<ApiResponse<List<ExamSheetDto>>> GetExamSheetAsync(
        Guid tenantId,
        Guid teacherId,
        Guid examId
    )
    {
        var exam = await context
            .Exams.AsNoTracking()
            .FirstOrDefaultAsync(e =>
                e.Id == examId && e.TeacherId == teacherId && e.Teacher.TenantId == tenantId
            );

        if (exam == null)
        {
            return ApiResponse<List<ExamSheetDto>>.Fail("الإمتحان غير موجود");
        }

        // 1. Start Query
        var query = context.StudentGroups.AsNoTracking();

        // 2. Filter The Exam Type
        if (exam.GroupId.HasValue)
        {
            query = query.Where(sg => sg.GroupId == exam.GroupId.Value);
        }
        else
        {
            query = query.Where(sg =>
                sg.Group.TeacherId == teacherId && sg.Group.Grade == exam.Grade
            );
        }

        // Get Exam Sheet
        var examSheet = await query
            .Select(sg => new
            {
                StudentId = sg.StudentId,
                StudentName = sg.Student.Name,
                Score = sg
                    .Student.ExamResults.Where(er => er.ExamId == examId)
                    .Select(er => (decimal?)er.Score)
                    .FirstOrDefault(),
            })
            .Select(x => new ExamSheetDto(
                x.StudentId,
                x.StudentName,
                x.Score,
                x.Score.HasValue ? (x.Score.Value / exam.MaxScore) * 100 : null
            ))
            .ToListAsync();

        return ApiResponse<List<ExamSheetDto>>.Ok(examSheet, "تم إرجاع شيت الإمتحان بنجاح.");
    }

    public async Task<ApiResponse<object>> SaveBulkExamSheetAsync(
        Guid tenantId,
        Guid teacherId,
        Guid examId,
        UpdateExamSheetDto dto
    )
    {
        // 1. Fetch exam metadata for validation
        var exam = await context
            .Exams.AsNoTracking()
            .FirstOrDefaultAsync(e =>
                e.Id == examId && e.TeacherId == teacherId && e.Teacher.TenantId == tenantId
            );

        if (exam == null)
        {
            return ApiResponse<object>.Fail("الإمتحان غير موجود أو لا تملك صلاحية الوصول إليه.");
        }

        // 2. Ensure no score exceeds the maximum allowed or drops below zero
        var hasInvalidScores = dto.StudentsScores.Any(sc =>
            sc.Score.HasValue && (sc.Score.Value > exam.MaxScore || sc.Score.Value < 0)
        );

        if (hasInvalidScores)
        {
            return ApiResponse<object>.Fail(
                $"يوجد درجات غير صحيحة! الدرجة يجب أن تكون بين 0 و {exam.MaxScore}"
            );
        }

        // 3. Load existing results into a Dictionary
        var existingResults = await context
            .ExamResults.Where(er => er.ExamId == examId)
            .ToDictionaryAsync(er => er.StudentId);

        // 4. Process the bulk operation
        foreach (var studentScore in dto.StudentsScores)
        {
            bool exists = existingResults.TryGetValue(
                studentScore.StudentId,
                out var existingRecord
            );

            if (!exists && studentScore.Score.HasValue)
            {
                // Student has no existing record => Insert
                context.ExamResults.Add(
                    new ExamResult
                    {
                        ExamId = examId,
                        StudentId = studentScore.StudentId,
                        Score = studentScore.Score.Value,
                    }
                );
            }
            else if (exists && studentScore.Score.HasValue)
            {
                // Student has an existing record and a new score is provided => Update
                existingRecord!.Score = studentScore.Score.Value;
            }
            else if (exists && !studentScore.Score.HasValue)
            {
                // Student has an existing record but the score is nullified => Delete
                context.ExamResults.Remove(existingRecord!);
            }
        }

        // 5. Commit all tracked changes to the database in a single transaction
        await context.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { }, "تم حفظ الشيت بنجاح.");
    }
}
