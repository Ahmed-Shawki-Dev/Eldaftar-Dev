using System.ComponentModel.DataAnnotations;
using api.Models;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace api.DTOs;

public record CreateScheduleDto(
    [Required] DayOfWeekEnum DayOfWeek,
    [Required] string StartTime,
    [Required] string EndTime
);

public record CreateGroupDto(
    [Required(ErrorMessage = "الصف الدراسي مطلوب")] string Grade,
    [Required(ErrorMessage = "اسم المجموعة مطلوب")] string Name,
    [Range(0, 10000, ErrorMessage = "السعر يجب أن يكون قيمة موجبة")] decimal Price,
    [Required(ErrorMessage = "يجب تحديد نوع الدفع")] PaymentType PaymentType,
    Guid? AcademicTermId,
    [MinLength(1, ErrorMessage = "يجب تحديد ميعاد واحد على الأقل للمجموعة")]
        List<CreateScheduleDto> Schedules
);

public record GroupSummaryDto(
    Guid Id,
    string Grade,
    string Name,
    decimal Price,
    PaymentType PaymentType,
    List<CreateScheduleDto> Schedules
);

public record GroupParamsDto
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
}
