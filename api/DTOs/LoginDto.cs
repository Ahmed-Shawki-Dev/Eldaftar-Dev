using System.ComponentModel.DataAnnotations;

namespace api.DTOs;

public record LoginDto(
    [Required, Phone] string PhoneNumber,
    [Required, MinLength(6)] string Password
);

public record LoginResponseDto(string Token, CurrentUserDto User);

public record CurrentUserDto(
    Guid UserId,
    string Role,
    string TenantSlug,
    Guid TenantId,
    Guid? TeacherId
);
