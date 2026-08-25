using api.DTOs;
using api.Models;

namespace api.Mappers;

public static class ExamMappingExtensions
{
    public static ExamDto ToExamDto(this Exam exam)
    {
        return new ExamDto(
            exam.Id,
            exam.Title,
            exam.Grade,
            exam.MaxScore,
            exam.Date,
            exam.GroupId,
            exam.Group?.Name,
            exam.CreatedAt
        );
    }

    public static List<ExamDto> ToExamDtoList(this IEnumerable<Exam> exams)
    {
        return exams.Select(e => e.ToExamDto()).ToList();
    }
}
