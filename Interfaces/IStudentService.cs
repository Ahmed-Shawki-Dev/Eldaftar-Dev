using api.DTOs;

namespace api.Interfaces;

public interface IStudentService
{
    Task<ApiResponse<StudentDto>> CreateStudentAsync(
        Guid tenantId,
        Guid teacherId,
        CreateStudentDto dto
    );
    Task<ApiResponse<List<StudentDto>>> GetAllStudentsAsync(
        Guid tenantId,
        Guid teacherId,
        StudentParamsDto parameters
    );
}
