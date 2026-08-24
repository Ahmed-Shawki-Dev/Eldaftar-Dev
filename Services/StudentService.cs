using api.Data;
using api.DTOs;
using api.Helpers;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class StudentService(ApplicationDBContext context) : IStudentService
{
    public async Task<ApiResponse<StudentDto>> CreateStudentAsync(
        Guid tenantId,
        Guid teacherId,
        CreateStudentDto dto
    )
    {
        // 1. Verify group ownership and existence within tenant
        var group = await context.Groups.FirstOrDefaultAsync(g =>
            g.Id == dto.GroupId && g.TeacherId == teacherId && g.TenantId == tenantId
        );

        if (group == null)
            return ApiResponse<StudentDto>.Fail("المجموعة غير موجودة أو لا تتبع هذا المدرس.");

        Student? student = null;

        // 2. Lookup existing student by phone if provided
        if (!string.IsNullOrWhiteSpace(dto.Phone))
        {
            var cleanPhone = dto.Phone.Trim();
            student = await context
                .Students.IgnoreQueryFilters()
                .Include(s => s.StudentGroups)
                .FirstOrDefaultAsync(s =>
                    s.TenantId == tenantId && s.Phone == cleanPhone && !s.IsDeleted
                );
        }

        // 3. Lookup by ParentPhone and Name if missing student phone
        if (student == null)
        {
            var cleanParentPhone = dto.ParentPhone.Trim();
            var cleanName = dto.Name.Trim();

            student = await context
                .Students.IgnoreQueryFilters()
                .Include(s => s.StudentGroups)
                .FirstOrDefaultAsync(s =>
                    s.TenantId == tenantId
                    && s.ParentPhone == cleanParentPhone
                    && s.Name == cleanName
                    && !s.IsDeleted
                );
        }

        // 4. Handle existing student Tenant-wide profile found
        if (student != null)
        {
            var existingEnrollment = student.StudentGroups.FirstOrDefault(sg =>
                sg.GroupId == dto.GroupId
            );

            if (existingEnrollment != null)
            {
                if (existingEnrollment.Status == EnrollmentStatus.Active)
                    return ApiResponse<StudentDto>.Fail("الطالب مسجل بالفعل في هذه المجموعة.");

                // Reactivate archived enrollment with updated custom price
                existingEnrollment.Status = EnrollmentStatus.Active;
                existingEnrollment.CustomPrice = dto.CustomPrice;
            }
            else
            {
                // Enroll student into the new group
                student.StudentGroups.Add(
                    new StudentGroup
                    {
                        GroupId = dto.GroupId,
                        CustomPrice = dto.CustomPrice,
                        Status = EnrollmentStatus.Active,
                    }
                );
            }
        }
        // 5. Handle new student creation
        else
        {
            // Ensure unique 6 digit student code per tenant
            string studentCode;
            do
            {
                studentCode = StudentCodeGenerator.GenerateRandomStudentCode();
            } while (
                await context.Students.AnyAsync(s =>
                    s.TenantId == tenantId && s.StudentCode == studentCode
                )
            );

            student = dto.ToEntity(tenantId, studentCode);
            await context.Students.AddAsync(student);
        }

        await context.SaveChangesAsync();

        return ApiResponse<StudentDto>.Ok(student.ToDto(group), "تم تسجيل الطالب بنجاح.");
    }

    public async Task<ApiResponse<List<StudentDto>>> GetAllStudentsAsync(
        Guid tenantId,
        Guid teacherId,
        StudentParamsDto parameters
    )
    {
        // 1. Base Query
        var query = context
            .StudentGroups.AsNoTracking()
            .Where(sg => sg.Group.TeacherId == teacherId && sg.Group.TenantId == tenantId);

        // 2. Group Filter
        if (parameters.GroupId.HasValue)
        {
            query = query.Where(sg => sg.GroupId == parameters.GroupId.Value);
        }

        // 3. Search Filter
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var term = parameters.SearchTerm.Trim();
            query = query.Where(sg =>
                sg.Student.Name.Contains(term)
                || (sg.Student.Phone != null && sg.Student.Phone.Contains(term))
                || sg.Student.StudentCode.Contains(term)
                || sg.Student.ParentPhone.Contains(term)
            );
        }

        // 4. Total Count for Pagination Metadata
        var totalRecords = await query.CountAsync();

        // 5. Fetch Paged Data with Includes
        var studentGroups = await query
            .Include(sg => sg.Student)
            .Include(sg => sg.Group)
            .OrderByDescending(sg => sg.CreatedAt)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        // 6. Map to DTOs
        var students = studentGroups.ToDtos();

        // 7. Calculate Pagination Metadata
        var pagination = PaginationMetadata.Create(
            parameters.PageNumber,
            parameters.PageSize,
            totalRecords
        );

        return ApiResponse<List<StudentDto>>.OkPaged(students, pagination, "تم جلب الطلاب بنجاح.");
    }

    public async Task<ApiResponse<StudentDto>> GetStudentByIdAsync(
        Guid tenantId,
        Guid teacherId,
        Guid studentId
    )
    {
        // * Get Student Enrollment With Teacher
        var studentGroup = await context
            .StudentGroups.AsNoTracking()
            .Include(sg => sg.Student)
            .Include(sg => sg.Group)
            .FirstOrDefaultAsync(sg =>
                sg.StudentId == studentId
                && sg.Group.TeacherId == teacherId
                && sg.Group.TenantId == tenantId
            );
        // * Check Student
        if (studentGroup == null)
        {
            return ApiResponse<StudentDto>.Fail("الطالب غير موجود أو لا يتبع هذا المدرس.");
        }

        // * Convert To Dto
        var studentDto = studentGroup.ToDto();
        return ApiResponse<StudentDto>.Ok(studentDto, "تم جلب بيانات الطالب بنجاح.");
    }

    public async Task<ApiResponse<StudentDto>> UpdateStudentAsync(
        Guid tenantId,
        Guid teacherId,
        Guid studentId,
        UpdateStudentDto dto
    )
    {
        // 1. Fetch Student Enrollment with Tracking
        var studentGroup = await context
            .StudentGroups.Include(sg => sg.Student)
            .Include(sg => sg.Group)
            .FirstOrDefaultAsync(sg =>
                sg.StudentId == studentId
                && sg.Group.TeacherId == teacherId
                && sg.Group.TenantId == tenantId
            );

        if (studentGroup == null)
        {
            return ApiResponse<StudentDto>.Fail("الطالب غير موجود أو لا ينتمي لهذا المدرس.");
        }

        var student = studentGroup.Student;

        // 2. Check Phone Uniqueness Only If Phone Actually Changed
        var cleanPhone = !string.IsNullOrWhiteSpace(dto.Phone) ? dto.Phone.Trim() : null;
        if (!string.IsNullOrWhiteSpace(cleanPhone) && cleanPhone != student.Phone)
        {
            var isPhoneExist = await context.Students.AnyAsync(s =>
                s.TenantId == tenantId && s.Phone == cleanPhone && s.Id != studentId
            );

            if (isPhoneExist)
            {
                return ApiResponse<StudentDto>.Fail("رقم هاتف الطالب مسجل مسبقاً لطالب آخر.");
            }
        }

        // 3. Verify New Group Ownership If Group Changed
        if (dto.GroupId != studentGroup.GroupId)
        {
            var targetGroup = await context.Groups.FirstOrDefaultAsync(g =>
                g.Id == dto.GroupId && g.TeacherId == teacherId && g.TenantId == tenantId
            );

            if (targetGroup == null)
            {
                return ApiResponse<StudentDto>.Fail(
                    "المجموعة الجديدة غير صالحة أو لا تتبع هذا المدرس."
                );
            }

            studentGroup.GroupId = dto.GroupId;
            studentGroup.Group = targetGroup;
        }

        // 4. Update Student & Enrollment Properties
        student.Name = dto.Name.Trim();
        student.ParentPhone = dto.ParentPhone.Trim();
        student.Phone = cleanPhone;
        studentGroup.CustomPrice = dto.CustomPrice;

        // 5. EF Core Change Tracker
        await context.SaveChangesAsync();

        return ApiResponse<StudentDto>.Ok(studentGroup.ToDto(), "تم تحديث بيانات الطالب بنجاح.");
    }
}
