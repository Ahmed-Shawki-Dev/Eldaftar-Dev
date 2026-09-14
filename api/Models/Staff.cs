using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

public class Staff : BaseModel
{
    public string Name { get; set; } = string.Empty;

    // * Teacher (one to many)
    public Guid TeacherId { get; set; }
    public Teacher Teacher { get; set; } = null!;

    // * AppUser (one-one)
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public AppUser User { get; set; } = null!;
}
