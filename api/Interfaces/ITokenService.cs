using api.Models;

namespace api.Interfaces;

public interface ITokenService
{
    Task<string> CreateTokenAsync(AppUser user, Tenant tenant);
}
