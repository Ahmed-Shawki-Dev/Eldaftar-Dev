using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Data;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace api.Services;

public class TokenService(
    IConfiguration config,
    UserManager<AppUser> userManager,
    ApplicationDBContext context
) : ITokenService
{
    public async Task<string> CreateTokenAsync(AppUser user, Tenant tenant)
    {
        var claims = new List<Claim>
        {
            new("userId", user.Id.ToString()),
            new("phone", user.PhoneNumber ?? string.Empty),
            new("tenantId", tenant.Id.ToString()),
            new("tenantSlug", tenant.Slug),
            new("tenantType", tenant.Type.ToString()),
        };

        var roles = await userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        if (roles.Contains("Teacher"))
        {
            var teacherId = await context
                .Teachers.Where(t => t.UserId == user.Id && !t.IsDeleted)
                .Select(t => t.Id)
                .FirstOrDefaultAsync();

            if (teacherId != Guid.Empty)
            {
                claims.Add(new Claim("teacherId", teacherId.ToString()));
            }
        }
        else if (roles.Contains("Staff"))
        {
            var teacherId = await context
                .Staffs.Where(s => s.UserId == user.Id)
                .Select(s => s.TeacherId)
                .FirstOrDefaultAsync();

            if (teacherId != Guid.Empty)
            {
                claims.Add(new Claim("teacherId", teacherId.ToString()));
            }
        }

        var secretKey =
            config["JWT:Secret"]
            ?? throw new InvalidOperationException("JWT:Secret is not configured");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = config["JWT:Issuer"],
            Audience = config["JWT:Audience"],
            SigningCredentials = creds,
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
