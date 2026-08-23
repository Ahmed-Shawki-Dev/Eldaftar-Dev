using api.DTOs;
using api.Models;

namespace api.Mappers;

public static class GroupMapper
{
    // Schedule Mappings
    public static CreateScheduleDto ToDto(this GroupSchedule schedule)
    {
        return new CreateScheduleDto(schedule.DayOfWeek, schedule.StartTime, schedule.EndTime);
    }

    public static GroupSchedule ToEntity(this CreateScheduleDto dto, Guid groupId = default)
    {
        return new GroupSchedule
        {
            GroupId = groupId,
            DayOfWeek = dto.DayOfWeek,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
        };
    }

    public static List<GroupSchedule> ToEntities(
        this IEnumerable<CreateScheduleDto> dtos,
        Guid groupId = default
    )
    {
        return dtos.Select(d => d.ToEntity(groupId)).ToList();
    }

    public static List<CreateScheduleDto> ToDtos(this IEnumerable<GroupSchedule> schedules)
    {
        return schedules.Select(s => s.ToDto()).ToList();
    }

    // Group Creation Mapping
    public static Group ToEntity(this CreateGroupDto dto, Guid teacherId, Guid tenantId)
    {
        return new Group
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
            Schedules = dto.Schedules.ToEntities(),
        };
    }

    public static GroupSummaryDto ToSummaryDto(this Group group)
    {
        return new GroupSummaryDto(
            group.Id,
            group.Grade,
            group.Name,
            group.Price,
            group.PaymentType,
            group.Schedules.ToDtos()
        );
    }
}
