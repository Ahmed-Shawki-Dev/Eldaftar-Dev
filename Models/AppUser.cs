using Microsoft.AspNetCore.Identity;

namespace api.Models;

public class AppUser : IdentityUser<Guid>
{
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
    public Teacher? Teacher { get; set; }
    public Staff? Staff { get; set; }
}
