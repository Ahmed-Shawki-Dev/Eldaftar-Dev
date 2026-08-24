using api.DTOs;
using api.Models;

namespace api.Mappers;

public static class StudentMapper
{
    // Mapping: CreateStudentDto -> Student Entity With Initial Enrollment
    public static Student ToEntity(this CreateStudentDto dto, Guid tenantId, string studentCode)
    {
        return new Student
        {
            TenantId = tenantId,
            StudentCode = studentCode,
            Name = dto.Name.Trim(),
            ParentPhone = dto.ParentPhone.Trim(),
            Phone = !string.IsNullOrWhiteSpace(dto.Phone) ? dto.Phone.Trim() : null,
            StudentGroups = new List<StudentGroup>
            {
                new StudentGroup
                {
                    GroupId = dto.GroupId,
                    CustomPrice = dto.CustomPrice,
                    Status = EnrollmentStatus.Active,
                },
            },
        };
    }

    // Student Entity + Group -> StudentDto
    public static StudentDto ToDto(this Student student, Group group)
    {
        return new StudentDto(
            student.Id,
            student.StudentCode,
            student.Name,
            student.ParentPhone,
            student.Phone,
            group.Id,
            $"{group.Grade} - {group.Name}"
        );
    }

    public static StudentDto ToDto(this StudentGroup sg)
    {
        return new StudentDto(
            sg.Student.Id,
            sg.Student.StudentCode,
            sg.Student.Name,
            sg.Student.ParentPhone,
            sg.Student.Phone,
            sg.Group.Id,
            $"{sg.Group.Grade} - {sg.Group.Name}"
        );
    }

    public static List<StudentDto> ToDtos(this List<StudentGroup> studentGroups)
    {
        return studentGroups.Select(sg => sg.ToDto()).ToList();
    }
}
