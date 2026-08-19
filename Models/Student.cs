namespace api.Models;

public class Student : BaseModel
{
    public required string StudentCode { get; set; }
    public required string Name { get; set; }
    public required string ParentPhone { get; set; }
    public string? Phone { get; set; }

    // * Soft Delete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // * Center Relation (1-M)
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    // ** Student Group
    public ICollection<StudentGroup> StudentGroups { get; set; } = new List<StudentGroup>();

    // ** Exam Results
    public ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
}
