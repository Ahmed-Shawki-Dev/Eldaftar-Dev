using System.ComponentModel.DataAnnotations;
using api.Models;

namespace api.DTOs;

public record RegisterTenantDto(
    [property: Required, Phone] string PhoneNumber,
    [property: Required, MinLength(6)] string Password,
    [property: Required] string FullName,
    [property: Required] string CenterName,
    [property:
        Required,
        RegularExpression(
            @"^[a-z0-9-]+$",
            ErrorMessage = "Slug must contain only lowercase letters, numbers, and hyphens"
        )
    ]
        string Slug,
    TenantType TenantType = TenantType.SoloTeacher
);
