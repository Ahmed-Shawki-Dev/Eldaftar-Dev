using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

public class Tenant : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    // * Relations
    public Subscription? Subscription { get; set; }
    public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

    // * AppUser (one-one)
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public AppUser User { get; set; } = null!;
}
