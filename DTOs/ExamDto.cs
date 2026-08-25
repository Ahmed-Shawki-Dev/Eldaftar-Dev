using System.ComponentModel.DataAnnotations;

namespace api.DTOs;

public record CreateExamDto(
    [Required(ErrorMessage = "عنوان الامتحان مطلوب")]
    [MaxLength(150, ErrorMessage = "العنوان لا يتجاوز 150 حرف")]
        string Title,
    [Required(ErrorMessage = "المرحلة الدراسية مطلوبة")] string Grade,
    [Required(ErrorMessage = "الدرجة العظمى مطلوبة")]
    [Range(1, 1000, ErrorMessage = "الدرجة العظمى يجب أن تكون أكبر من 0")]
        decimal MaxScore,
    DateTimeOffset? Date,
    Guid? GroupId
);

public record UpdateExamDto(
    [Required(ErrorMessage = "عنوان الامتحان مطلوب")]
    [MaxLength(150, ErrorMessage = "العنوان لا يتجاوز 150 حرف")]
        string Title,
    [Required(ErrorMessage = "المرحلة الدراسية مطلوبة")] string Grade,
    [Required(ErrorMessage = "الدرجة العظمى مطلوبة")]
    [Range(1, 1000, ErrorMessage = "الدرجة العظمى يجب أن تكون أكبر من 0")]
        decimal MaxScore,
    DateTimeOffset Date,
    Guid? GroupId
);

public record ExamDto(
    Guid Id,
    string Title,
    string Grade,
    decimal MaxScore,
    DateTimeOffset Date,
    Guid? GroupId,
    string? GroupName,
    DateTimeOffset CreatedAt
);

public record ExamParamsDto
{
    private const int MaxPageSize = 50;

    private int _pageNumber = 1;
    public int PageNumber
    {
        get => _pageNumber;
        init => _pageNumber = value < 1 ? 1 : value;
    }

    private int _pageSize = 10;
    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > MaxPageSize ? MaxPageSize : (value < 1 ? 10 : value);
    }

    public string? SearchTerm { get; init; }
    public string? Grade { get; init; }
    public Guid? GroupId { get; init; }
}
