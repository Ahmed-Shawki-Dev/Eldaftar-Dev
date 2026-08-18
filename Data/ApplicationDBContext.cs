using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Staff> Staffs { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // 1. Unique Slug Centers
        builder.Entity<Tenant>().HasIndex(t => t.Slug).IsUnique();

        // 2. Enum As String
        builder.Entity<Subscription>().Property(s => s.Plan).HasConversion<string>();
        builder.Entity<Tenant>().Property(t => t.Type).HasConversion<string>();

        // 3. Dont repeat phone number at same tenant
        builder.Entity<AppUser>().HasIndex(u => new { u.TenantId, u.PhoneNumber }).IsUnique();

        // 4. Dont repeat the teacher at same center
        builder.Entity<Teacher>().HasIndex(t => new { t.TenantId, t.UserId }).IsUnique();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }
}
