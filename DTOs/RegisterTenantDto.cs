using System.ComponentModel.DataAnnotations;
using api.Models;

namespace api.DTOs;

public record RegisterTenantDto(
    [Required, Phone] string PhoneNumber,
    [Required, MinLength(6)] string Password,
    [Required] string FullName,
    [Required] string CenterName,
    [
        Required,
        RegularExpression(
            @"^[a-z0-9-]+$",
            ErrorMessage = "Slug must contain only lowercase letters, numbers, and hyphens"
        )
    ]
        string Slug,
    TenantType TenantType = TenantType.SoloTeacher
);

public record TenantAuthResponseDto(
    Guid TenantId,
    string CenterName,
    string Slug,
    Guid UserId,
    string PhoneNumber,
    string FullName
);
