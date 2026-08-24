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
}
