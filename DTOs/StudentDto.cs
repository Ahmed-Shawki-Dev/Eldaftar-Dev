using System.ComponentModel.DataAnnotations;
using api.Models;

namespace api.DTOs;

public record CreateStudentDto(
    [Required(ErrorMessage = "اسم الطالب مطلوب")]
    [MaxLength(100, ErrorMessage = "اسم الطالب لا يجب أن يتجاوز 100 حرف")]
        string Name,
    [Required(ErrorMessage = "رقم ولي الأمر مطلوب")]
    [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "صيغة رقم هاتف ولي الأمر غير صحيحة")]
        string ParentPhone,
    [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "صيغة رقم هاتف الطالب غير صحيحة")]
        string? Phone,
    [Required(ErrorMessage = "يجب اختيار المجموعة")] Guid GroupId,
    [Range(0, 10000, ErrorMessage = "السعر الخاص يجب أن يكون قيمة موجبة")] decimal? CustomPrice
);

public record UpdateStudentDto(
    [Required(ErrorMessage = "اسم الطالب مطلوب")]
    [MaxLength(100, ErrorMessage = "اسم الطالب لا يجب أن يتجاوز 100 حرف")]
        string Name,
    [Required(ErrorMessage = "رقم ولي الأمر مطلوب")]
    [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "صيغة رقم هاتف ولي الأمر غير صحيحة")]
        string ParentPhone,
    [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "صيغة رقم هاتف الطالب غير صحيحة")]
        string? Phone,
    [Required(ErrorMessage = "يجب تحديد المجموعة")] Guid GroupId,
    [Range(0, 10000, ErrorMessage = "السعر الخاص يجب أن يكون قيمة موجبة")] decimal? CustomPrice
);

public record StudentDto(
    Guid Id,
    string StudentCode,
    string Name,
    string ParentPhone,
    string? Phone,
    Guid GroupId,
    string GroupName
);

public record StudentParamsDto
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
    public Guid? GroupId { get; init; }
}
