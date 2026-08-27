using api.DTOs;
using api.Helpers;

namespace api.Interfaces;

public interface IExamService
{
    Task<ApiResponse<ExamDto>> CreateExamAsync(Guid tenantId, Guid teacherId, CreateExamDto dto);

    Task<ApiResponse<List<ExamDto>>> GetAllExamsAsync(
        Guid tenantId,
        Guid teacherId,
        ExamParamsDto parameters
    );

    Task<ApiResponse<ExamDto>> GetExamByIdAsync(Guid tenantId, Guid teacherId, Guid examId);

    Task<ApiResponse<List<ExamSheetDto>>> GetExamSheetAsync(
        Guid tenantId,
        Guid teacherId,
        Guid examId
    );

    Task<ApiResponse<object>> SaveBulkExamSheetAsync(
        Guid tenantId,
        Guid teacherId,
        Guid examId,
        UpdateExamSheetDto dto
    );

    Task<ApiResponse<ExamDto>> UpdateExamAsync(
        Guid tenantId,
        Guid teacherId,
        Guid examId,
        UpdateExamDto dto
    );

    Task<ApiResponse<object>> DeleteExamAsync(Guid tenantId, Guid teacherId, Guid examId);
}
