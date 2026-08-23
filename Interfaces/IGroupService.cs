using api.DTOs;

namespace api.Interfaces;

public interface IGroupService
{
    Task<ApiResponse<GroupSummaryDto>> CreateGroupAsync(Guid teacherId, CreateGroupDto dto);
    Task<ApiResponse<GroupDetailsDto>> GetGroupByIdAsync(Guid teacherId, Guid groupId);
    Task<ApiResponse<List<GroupSummaryDto>>> GetGroupsAsync(
        Guid teacherId,
        GroupFilterParams filters
    );
    Task<ApiResponse<GroupSummaryDto>> UpdateGroupAsync(
        Guid teacherId,
        Guid groupId,
        UpdateGroupDto dto
    );
    Task<ApiResponse<string>> SoftDeleteGroupAsync(Guid teacherId, Guid groupId);
}
