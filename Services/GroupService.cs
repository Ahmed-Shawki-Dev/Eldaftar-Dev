using api.Data;
using api.DTOs;
using api.Helpers;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class GroupService(ApplicationDBContext context) : IGroupService
{
    // * Create New Group
    public async Task<ApiResponse<GroupSummaryDto>> CreateGroupAsync(
        Guid teacherId,
        CreateGroupDto dto
    )
    {
        // 1. Check If The Group Exist
        var existingGroup = await context
            .Groups.IgnoreQueryFilters()
            .Include(g => g.Schedules)
            .FirstOrDefaultAsync(g =>
                g.TeacherId == teacherId && g.Grade == dto.Grade && g.Name == dto.Name
            );

        Group groupToReturn;

        // 2. Check If Group Already Active Or Needs Restore
        if (existingGroup != null)
        {
            if (!existingGroup.IsDeleted)
            {
                return ApiResponse<GroupSummaryDto>.Fail(
                    "يوجد مجموعة نشطة بالفعل بنفس الاسم لهذه المرحلة."
                );
            }

            // Restore Soft-Deleted Group
            existingGroup.Price = dto.Price;
            existingGroup.PaymentType = dto.PaymentType;
            existingGroup.AcademicTermId = dto.AcademicTermId;
            existingGroup.IsDeleted = false;
            existingGroup.DeletedAt = null;
            existingGroup.IsActive = true;

            existingGroup.Schedules.Clear();
            existingGroup.Schedules = dto
                .Schedules.Select(s => new GroupSchedule
                {
                    DayOfWeek = s.DayOfWeek,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                })
                .ToList();

            groupToReturn = existingGroup;
        }
        else
        {
            // 3. Create New Group Instance
            var tenantId = await context
                .Teachers.Where(t => t.Id == teacherId)
                .Select(t => t.TenantId)
                .FirstOrDefaultAsync();

            if (tenantId == Guid.Empty)
            {
                return ApiResponse<GroupSummaryDto>.Fail(
                    "تعذر العثور على بيانات المدرس أو السنتر التابع له."
                );
            }

            var newGroup = new Group
            {
                TenantId = tenantId,
                TeacherId = teacherId,
                Grade = dto.Grade,
                Name = dto.Name,
                Price = dto.Price,
                PaymentType = dto.PaymentType,
                AcademicTermId = dto.AcademicTermId,
                IsActive = true,
                IsDeleted = false,
                Schedules = dto
                    .Schedules.Select(s => new GroupSchedule
                    {
                        DayOfWeek = s.DayOfWeek,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime,
                    })
                    .ToList(),
            };

            await context.Groups.AddAsync(newGroup);
            groupToReturn = newGroup;
        }

        // 4. Save Changes
        await context.SaveChangesAsync();

        // 5. Map And Return Summary DTO
        var responseDto = new GroupSummaryDto(
            groupToReturn.Id,
            groupToReturn.Grade,
            groupToReturn.Name,
            groupToReturn.Price,
            0,
            groupToReturn.PaymentType,
            groupToReturn
                .Schedules.Select(s => new CreateScheduleDto(s.DayOfWeek, s.StartTime, s.EndTime))
                .ToList()
        );

        return ApiResponse<GroupSummaryDto>.Ok(responseDto, "تم إنشاء المجموعة بنجاح.");
    }

    // * Show Group By Id
    public async Task<ApiResponse<GroupDetailsDto>> GetGroupByIdAsync(Guid teacherId, Guid groupId)
    {
        // 1. Fetch Group Details with Nested Projections
        var groupDto = await context
            .Groups.AsNoTracking()
            .Where(g => g.Id == groupId && g.TeacherId == teacherId)
            .Select(g => new GroupDetailsDto(
                g.Id,
                g.Grade,
                g.Name,
                g.Price,
                g.PaymentType,
                g.AcademicTermId,
                g.Schedules.Select(s => new CreateScheduleDto(s.DayOfWeek, s.StartTime, s.EndTime))
                    .ToList(),
                g.StudentGroups.Select(sg => new GroupStudentDto(
                        sg.StudentId,
                        sg.Student.StudentCode,
                        sg.Student.Name,
                        sg.Student.ParentPhone,
                        sg.Student.Phone,
                        sg.Status,
                        sg.CustomPrice
                    ))
                    .ToList()
            ))
            .FirstOrDefaultAsync();

        // 2. Check Existence
        if (groupDto == null)
        {
            return ApiResponse<GroupDetailsDto>.Fail("المجموعة غير موجودة.");
        }

        // 3. Return Success Response
        return ApiResponse<GroupDetailsDto>.Ok(groupDto);
    }

    public Task<ApiResponse<List<GroupSummaryDto>>> GetGroupsAsync(
        Guid teacherId,
        GroupFilterParams filters
    )
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<string>> SoftDeleteGroupAsync(Guid teacherId, Guid groupId)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<GroupSummaryDto>> UpdateGroupAsync(
        Guid teacherId,
        Guid groupId,
        UpdateGroupDto dto
    )
    {
        throw new NotImplementedException();
    }
}
