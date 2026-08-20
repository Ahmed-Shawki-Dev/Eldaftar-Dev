namespace api.Models;

public enum AttendanceStatus
{
    Absent = 1,
    Present = 2,
    Excused = 3,
}

public class Attendance : BaseModel
{
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Absent;
    public bool IsMakeup { get; set; } = false;
    public string? Note { get; set; }

    // Relations
    public Guid SessionId { get; set; }
    public Session Session { get; set; } = null!;

    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;
}
