namespace api.Models;

public class ExamResult : BaseModel
{
    public decimal Score { get; set; }
    public string? Note { get; set; }

    // * Relations
    // 1. Exam
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    // 2. Student
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;
}
