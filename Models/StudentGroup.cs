namespace api.Models;

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
    public bool IsActive { get; set; } = true;
}
