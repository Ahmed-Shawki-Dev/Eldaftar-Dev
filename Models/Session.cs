namespace api.Models;

public enum SessionStatus
{
    Scheduled = 1,
    Completed = 2,
    Canceled = 3,
}

public class Session : BaseModel
{
    public DateTimeOffset SessionDate { get; set; } = DateTimeOffset.UtcNow;
    public SessionStatus Status { get; set; } = SessionStatus.Scheduled;
    public string? Note { get; set; }

    // Relations
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;

    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}
