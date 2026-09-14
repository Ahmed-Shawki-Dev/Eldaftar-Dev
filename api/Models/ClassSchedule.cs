namespace api.Models;

public enum DayOfWeekEnum
{
    Sunday = 0,
    Monday = 1,
    Tuesday = 2,
    Wednesday = 3,
    Thursday = 4,
    Friday = 5,
    Saturday = 6,
}

public class GroupSchedule : BaseModel
{
    public DayOfWeekEnum DayOfWeek { get; set; }
    public required string StartTime { get; set; }
    public required string EndTime { get; set; }

    // Relations
    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;
}
