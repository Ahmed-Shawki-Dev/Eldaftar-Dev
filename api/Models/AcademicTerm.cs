namespace api.Models;

public enum TermType
{
    Semester = 1,
    FullYear = 2,
}

public class AcademicTerm : BaseModel
{
    public required string Name { get; set; }
    public TermType Type { get; set; } = TermType.Semester;

    public bool IsActive { get; set; } = true;

    public DateTimeOffset? StartDate { get; set; }
    public DateTimeOffset? EndDate { get; set; }

    // * Soft Delete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // * Relations
    // 1. Center
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    // 2. Groups
    public ICollection<Group> Groups { get; set; } = new List<Group>();
}
