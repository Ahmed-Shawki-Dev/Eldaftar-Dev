using System.ComponentModel.DataAnnotations;

namespace api.DTOs;

public record CreateStaffDto(
    [Required(ErrorMessage = "اسم السكرتيرة مطلوب")]
    [MaxLength(100, ErrorMessage = "الاسم لا يتجاوز 100 حرف")]
        string Name,
    [Required(ErrorMessage = "رقم الهاتف مطلوب")]
    [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "صيغة رقم الهاتف غير صحيحة")]
        string PhoneNumber,
    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    [MinLength(6, ErrorMessage = "كلمة المرور لا تقل عن 6 أحرف")]
        string Password
);

public record UpdateStaffDto(
    [Required(ErrorMessage = "اسم السكرتيرة مطلوب")]
    [MaxLength(100, ErrorMessage = "الاسم لا يتجاوز 100 حرف")]
        string Name,
    [Required(ErrorMessage = "رقم الهاتف مطلوب")]
    [RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "صيغة رقم الهاتف غير صحيحة")]
        string PhoneNumber,
    [MinLength(6, ErrorMessage = "كلمة المرور لا تقل عن 6 أحرف")] string? NewPassword
);

public record StaffDto(
    Guid Id,
    string Name,
    string PhoneNumber,
    Guid TeacherId,
    DateTimeOffset CreatedAt
);
