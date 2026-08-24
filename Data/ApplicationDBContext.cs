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
    public DbSet<GroupSchedule> GroupSchedules { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Attendance> Attendances { get; set; }
    public DbSet<StudentInvoice> StudentInvoices { get; set; }
    public DbSet<PaymentTransaction> PaymentTransactions { get; set; }

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
        builder.Entity<GroupSchedule>().Property(c => c.DayOfWeek).HasConversion<string>();
        builder.Entity<Session>().Property(s => s.Status).HasConversion<string>();
        builder.Entity<Attendance>().Property(a => a.Status).HasConversion<string>();
        builder.Entity<StudentInvoice>().Property(i => i.Type).HasConversion<string>();
        builder.Entity<StudentInvoice>().Property(i => i.Status).HasConversion<string>();
        builder.Entity<PaymentTransaction>().Property(p => p.Method).HasConversion<string>();

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
        builder
            .Entity<StudentGroup>()
            .HasIndex(sg => new
            {
                sg.StudentId,
                sg.GroupId,
                sg.Status,
            });

        // 10. Dont repeat Student Mark In The Same Exam
        builder.Entity<ExamResult>().HasIndex(er => new { er.ExamId, er.StudentId }).IsUnique();

        // 11. Delete Exam Result When Delete Exma
        builder
            .Entity<Exam>()
            .HasMany(e => e.Results)
            .WithOne(r => r.Exam)
            .HasForeignKey(r => r.ExamId)
            .OnDelete(DeleteBehavior.Cascade);

        // 12. Don't Repeat Same Group With Start Date And Day
        builder
            .Entity<GroupSchedule>()
            .HasIndex(c => new
            {
                c.GroupId,
                c.DayOfWeek,
                c.StartTime,
            })
            .IsUnique();

        // 13. One Session Must Have One Student Id
        builder.Entity<Attendance>().HasIndex(a => new { a.SessionId, a.StudentId }).IsUnique();
        // 14. Index on SessionId To Fast DB Search
        builder.Entity<Attendance>().HasIndex(a => a.SessionId);
        // 15. Index on StudentId To Fast DB Search
        builder.Entity<Attendance>().HasIndex(a => a.StudentId);

        // 16. When Delete Group Delete There GroupSchedules
        builder
            .Entity<Group>()
            .HasMany(g => g.Schedules)
            .WithOne(s => s.Group)
            .HasForeignKey(s => s.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // 17. When Delete Group Delete There Sessions
        builder
            .Entity<Group>()
            .HasMany(g => g.Sessions)
            .WithOne(s => s.Group)
            .HasForeignKey(s => s.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // 18. When Delete Session Delete Attendances
        builder
            .Entity<Session>()
            .HasMany(s => s.Attendances)
            .WithOne(a => a.Session)
            .HasForeignKey(a => a.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        // 19. Delete Student Not Delete There Attendance
        builder
            .Entity<Student>()
            .HasMany(st => st.Attendances)
            .WithOne(a => a.Student)
            .HasForeignKey(a => a.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        // 20. Fast Monthly Payment Search
        builder.Entity<StudentInvoice>().HasIndex(i => new { i.GroupId, i.MonthKey });
        builder.Entity<StudentInvoice>().HasIndex(i => new { i.StudentId, i.Status });
        builder.Entity<StudentInvoice>().HasIndex(i => i.SessionId);
        builder.Entity<PaymentTransaction>().HasIndex(t => t.InvoiceId);
        builder.Entity<PaymentTransaction>().HasIndex(t => t.CreatedAt);
        builder.Entity<PaymentTransaction>().HasIndex(t => t.ReceivedByUserId);

        // 21. Dont repeat Invoice to same student on group in same month
        builder
            .Entity<StudentInvoice>()
            .HasIndex(i => new
            {
                i.StudentId,
                i.GroupId,
                i.MonthKey,
            })
            .IsUnique()
            .HasFilter("\"MonthKey\" IS NOT NULL");

        // 22. Dont Repeat Same Invoice To Student In Same Session
        builder
            .Entity<StudentInvoice>()
            .HasIndex(i => new { i.StudentId, i.SessionId })
            .IsUnique()
            .HasFilter("\"SessionId\" IS NOT NULL");

        // 23.
        builder
            .Entity<PaymentTransaction>()
            .HasOne(t => t.ReceivedByUser)
            .WithMany()
            .HasForeignKey(t => t.ReceivedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // 24. Delete AppUser When Delete Tenant
        builder
            .Entity<AppUser>()
            .HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        // ! Soft Delete Constrains
        builder.Entity<Student>().HasQueryFilter(s => !s.IsDeleted);
        builder.Entity<Group>().HasQueryFilter(g => !g.IsDeleted);
        builder.Entity<AcademicTerm>().HasQueryFilter(t => !t.IsDeleted);
        builder.Entity<Teacher>().HasQueryFilter(t => !t.IsDeleted);
        builder.Entity<StudentGroup>().HasQueryFilter(sg => sg.Status == EnrollmentStatus.Active);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }
}
