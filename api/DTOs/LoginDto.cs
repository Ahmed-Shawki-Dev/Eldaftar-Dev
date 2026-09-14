using System.ComponentModel.DataAnnotations;

namespace api.DTOs;

public record LoginDto(
    [Required, Phone] string PhoneNumber,
    [Required, MinLength(6)] string Password
);

public record LoginResponseDto(string Token);
