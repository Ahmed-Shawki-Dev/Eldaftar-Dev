using api.DTOs;

namespace api.Interfaces;

public interface IStudentService
{
    Task<ApiResponse<List<StudentDto>>> GetAllStudentsAsync(
        Guid tenantId,
        Guid teacherId,
        StudentParamsDto parameters
    );

    Task<ApiResponse<StudentDto>> GetStudentByIdAsync(
        Guid tenantId,
        Guid teacherId,
        Guid studentId
    );

    Task<ApiResponse<StudentDto>> CreateStudentAsync(
        Guid tenantId,
        Guid teacherId,
        CreateStudentDto dto
    );

    Task<ApiResponse<StudentDto>> UpdateStudentAsync(
        Guid tenantId,
        Guid teacherId,
        Guid StudentId,
        UpdateStudentDto dto
    );

    Task<ApiResponse<object>> SoftDeleteStudentAsync(Guid tenantId, Guid teacherId, Guid StudentId);
}
