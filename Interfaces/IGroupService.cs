using api.DTOs;

namespace api.Interfaces;

public interface IGroupService
{
    Task<ApiResponse<GroupSummaryDto>> CreateGroupAsync(
        Guid tenantId,
        Guid teacherId,
        CreateGroupDto dto
    );
}
