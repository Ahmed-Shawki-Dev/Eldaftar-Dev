using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

public class Teacher : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public string? Subject { get; set; }
    public string? Bio { get; set; }
    public string? PhotoUrl { get; set; }

    // * Soft Delete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // * Staff (one to many)
    public ICollection<Staff> Staffs { get; set; } = new List<Staff>();

    // * Center(one - many)
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    // * AppUser (one-one)
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public AppUser User { get; set; } = null!;

    // * Relation Group (1-M)
    public ICollection<Group> Groups { get; set; } = new List<Group>();

    // * Relation Exam (1-M)
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
