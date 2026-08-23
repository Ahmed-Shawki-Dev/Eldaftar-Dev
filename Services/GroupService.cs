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

    // * 2. Get Groub By Id
    public async Task<ApiResponse<GroupSummaryDto>> GetGroupByIdAsync(
        Guid tenantId,
        Guid teacherId,
        Guid groupId
    )
    {
        var group = await context
            .Groups.Include(g => g.Schedules)
            .AsNoTracking()
            .FirstOrDefaultAsync(g =>
                g.Id == groupId && g.TenantId == tenantId && g.TeacherId == teacherId
            );

        if (group == null)
        {
            return ApiResponse<GroupSummaryDto>.Fail("المجموعة غير موجودة.");
        }

        return ApiResponse<GroupSummaryDto>.Ok(
            group.ToSummaryDto(),
            "تم جلب بيانات المجموعة بنجاح."
        );
    }

    // * 3. Get All Groups
    public async Task<ApiResponse<List<GroupSummaryDto>>> GetGroupsAsync(
        Guid tenantId,
        Guid teacherId,
        GroupParamsDto parameters
    )
    {
        var query = context
            .Groups.Include(g => g.Schedules)
            .AsNoTracking()
            .Where(g => g.TenantId == tenantId && g.TeacherId == teacherId);

        // Search Filter With Grade Or Name
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var search = parameters.SearchTerm.Trim().ToLower();
            query = query.Where(g =>
                g.Name.ToLower().Contains(search) || g.Grade.ToLower().Contains(search)
            );
        }

        // 3. Grade Filter
        if (!string.IsNullOrWhiteSpace(parameters.Grade))
        {
            query = query.Where(g => g.Grade == parameters.Grade);
        }

        // 4. Count Total Records
        var totalRecords = await query.CountAsync();

        // 5. Apply Sorting & Pagination
        var groups = await query
            .OrderBy(g => g.Grade)
            .ThenBy(g => g.Name)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        // 6. Calculate Metadata
        var totalPages = (int)Math.Ceiling(totalRecords / (double)parameters.PageSize);
        var paginationMetadata = new PaginationMetadata(
            parameters.PageNumber,
            parameters.PageSize,
            totalPages,
            totalRecords
        );

        var groupsDto = groups.ToSummaryDtos();
        return ApiResponse<List<GroupSummaryDto>>.OkPaged(
            groupsDto,
            paginationMetadata,
            "تم جلب المجموعات بنجاح."
        );
    }
}
