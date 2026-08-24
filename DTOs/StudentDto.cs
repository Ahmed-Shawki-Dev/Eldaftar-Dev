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

public record StudentDto(
    Guid Id,
    string StudentCode,
    string Name,
    string ParentPhone,
    string? Phone,
    Guid GroupId,
    string GroupName
);
