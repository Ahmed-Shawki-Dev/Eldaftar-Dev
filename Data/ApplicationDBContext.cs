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
    public DbSet<AcademicTerm> AcademicTerms { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<StudentGroup> StudentGroups { get; set; }
    public DbSet<Exam> Exams { get; set; }
    public DbSet<ExamResult> ExamResults { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // 1. Unique Slug Centers
        builder.Entity<Tenant>().HasIndex(t => t.Slug).IsUnique();

        // 2. Enum As String
        builder.Entity<Subscription>().Property(s => s.Plan).HasConversion<string>();
        builder.Entity<Tenant>().Property(t => t.Type).HasConversion<string>();
        builder.Entity<Group>().Property(g => g.PaymentType).HasConversion<string>();
        builder.Entity<AcademicTerm>().Property(t => t.Type).HasConversion<string>();
        builder.Entity<StudentGroup>().Property(sg => sg.Status).HasConversion<string>();

        // 3. Dont repeat phone number at same tenant
        builder.Entity<AppUser>().HasIndex(u => new { u.TenantId, u.PhoneNumber }).IsUnique();

        // 4. Dont repeat the teacher at same center
        builder.Entity<Teacher>().HasIndex(t => new { t.TenantId, t.UserId }).IsUnique();

        // 5. Dont repeat StudentCode In The Same Center
        builder.Entity<Student>().HasIndex(s => new { s.TenantId, s.StudentCode }).IsUnique();

        // 6. Dont repeat Student Name And his ParentPhone In Same Center
        builder
            .Entity<Student>()
            .HasIndex(s => new
            {
                s.TenantId,
                s.Name,
                s.ParentPhone,
            })
            .IsUnique();

        // 7. Dont repeat Group Name To The Same Teacher
        builder
            .Entity<Group>()
            .HasIndex(g => new
            {
                g.TeacherId,
                g.Grade,
                g.Name,
            })
            .IsUnique();

        // 8. Dont repeat Term Name To The Same Tenant
        builder.Entity<AcademicTerm>().HasIndex(t => new { t.TenantId, t.Name }).IsUnique();

        // 9. Dont repeat Student In The Same Group
        builder.Entity<StudentGroup>().HasIndex(sg => new { sg.StudentId, sg.GroupId }).IsUnique();

        // 10. Dont repeat Student Mark In The Same Exam
        builder.Entity<ExamResult>().HasIndex(er => new { er.ExamId, er.StudentId }).IsUnique();

        // 11. Delete Exam Result When Delete Exma
        builder
            .Entity<Exam>()
            .HasMany(e => e.Results)
            .WithOne(r => r.Exam)
            .HasForeignKey(r => r.ExamId)
            .OnDelete(DeleteBehavior.Cascade);

        // ! Soft Delete Constrains
        builder.Entity<Student>().HasQueryFilter(s => !s.IsDeleted);
        builder.Entity<Group>().HasQueryFilter(g => !g.IsDeleted);
        builder.Entity<AcademicTerm>().HasQueryFilter(t => !t.IsDeleted);
        builder.Entity<Teacher>().HasQueryFilter(t => !t.IsDeleted);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }
}
