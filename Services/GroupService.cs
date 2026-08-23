using api.Data;
using api.DTOs;
using api.Interfaces;
using api.Mappers;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class GroupService(ApplicationDBContext context) : IGroupService
{
    // * 1. Create New Group
    public async Task<ApiResponse<GroupSummaryDto>> CreateGroupAsync(
        Guid tenantId,
        Guid teacherId,
        CreateGroupDto dto
    )
    {
        // 1. Check if group exists (including soft-deleted)
        var existingGroup = await context
            .Groups.IgnoreQueryFilters()
            .FirstOrDefaultAsync(g =>
                g.TeacherId == teacherId && g.Grade == dto.Grade && g.Name == dto.Name
            );

        Group groupToReturn;

        // 2. Handle active vs soft-deleted groups
        if (existingGroup != null)
        {
            if (!existingGroup.IsDeleted)
            {
                return ApiResponse<GroupSummaryDto>.Fail(
                    "يوجد مجموعة نشطة بالفعل بنفس الاسم لهذه المرحلة."
                );
            }

            // Restore and update group details
            existingGroup.Price = dto.Price;
            existingGroup.PaymentType = dto.PaymentType;
            existingGroup.AcademicTermId = dto.AcademicTermId;
            existingGroup.IsDeleted = false;
            existingGroup.DeletedAt = null;
            existingGroup.IsActive = true;

            // Fetch existing schedules and remove them explicitly
            var oldSchedules = await context
                .GroupSchedules.Where(s => s.GroupId == existingGroup.Id)
                .ToListAsync();

            context.GroupSchedules.RemoveRange(oldSchedules);

            // Map and attach new schedules
            var newSchedules = dto.Schedules.ToEntities(existingGroup.Id);
            await context.GroupSchedules.AddRangeAsync(newSchedules);

            existingGroup.Schedules = newSchedules;
            groupToReturn = existingGroup;
        }
        else
        {
            if (tenantId == Guid.Empty)
            {
                return ApiResponse<GroupSummaryDto>.Fail(
                    "تعذر العثور على بيانات المدرس أو السنتر التابع له."
                );
            }

            var newGroup = dto.ToEntity(teacherId, tenantId);
            await context.Groups.AddAsync(newGroup);
            groupToReturn = newGroup;
        }

        // 4. Commit transaction
        await context.SaveChangesAsync();

        // 5. Map and return response
        return ApiResponse<GroupSummaryDto>.Ok(
            groupToReturn.ToSummaryDto(),
            "تم إنشاء المجموعة بنجاح."
        );
    }
}
