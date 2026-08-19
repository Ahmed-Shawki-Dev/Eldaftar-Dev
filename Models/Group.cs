namespace api.Models;

public enum PaymentType
{
    PerSession = 1,
    Monthly = 2,
}

public class Group : BaseModel
{
    public required string Name { get; set; }
    public required string Grade { get; set; }
    public decimal Price { get; set; } = 0;
    public PaymentType PaymentType { get; set; } = PaymentType.PerSession;

    // * Soft Delete
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }

    // * Relations
    // 1. Teacher
    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;

    // 2. Tenant
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    // 3. Academic Term
    public Guid? AcademicTermId { get; set; }
    public AcademicTerm? AcademicTerm { get; set; }

    // 4. Student Group
    public ICollection<StudentGroup> StudentGroups { get; set; } = new List<StudentGroup>();

    // 4. Exam
    public ICollection<Exam> Exams { get; set; } = new List<Exam>();
}
