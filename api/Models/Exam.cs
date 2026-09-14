namespace api.Models;

public class Exam : BaseModel
{
    public required string Title { get; set; }
    public required string Grade { get; set; }
    public DateTimeOffset Date { get; set; } = DateTimeOffset.UtcNow;
    public decimal MaxScore { get; set; }

    // * Relations
    // 1. Teacher
    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;

    // 2. Group
    public Guid? GroupId { get; set; }
    public Group? Group { get; set; }

    // 3. Exam Results
    public ICollection<ExamResult> Results { get; set; } = new List<ExamResult>();
}
