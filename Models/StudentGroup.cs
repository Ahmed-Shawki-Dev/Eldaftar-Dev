namespace api.Models;

public enum EnrollmentStatus
{
    Pending = 1,
    Active = 2,
    Rejected = 3,
    Archived = 4,
}

public class StudentGroup : BaseModel
{
    // 1. Student
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    // 2. Group
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;

    // 3. Subscription Details
    public decimal? CustomPrice { get; set; }
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
}
