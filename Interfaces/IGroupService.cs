using api.DTOs;

namespace api.Interfaces;

public interface IGroupService
{
    Task<ApiResponse<GroupSummaryDto>> CreateGroupAsync(
        Guid tenantId,
        Guid teacherId,
        CreateGroupDto dto
    );

    Task<ApiResponse<List<GroupSummaryDto>>> GetGroupsAsync(
        Guid tenantId,
        Guid teacherId,
        GroupParamsDto parameters
    );

    Task<ApiResponse<GroupSummaryDto>> GetGroupByIdAsync(
        Guid tenantId,
        Guid teacherId,
        Guid groupId
    );
}
