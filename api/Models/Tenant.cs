using System.ComponentModel.DataAnnotations.Schema;

namespace api.Models;

public enum TenantType
{
    SoloTeacher,
    Center,
}

public class Tenant : BaseModel
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;

    // * Tenant Type
    public TenantType Type { get; set; } = TenantType.SoloTeacher;

    // * Relations
    public Subscription? Subscription { get; set; }
    public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();

    // * AppUser (one-one)
    public Guid? UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public AppUser? User { get; set; } = null!;

    // * Relation Student (1-M)
    public ICollection<Student> Students { get; set; } = new List<Student>();

    // * Relation Group (1-M)
    public ICollection<Group> Groups { get; set; } = new List<Group>();

    // * Relation Academic Term (1-M)
    public ICollection<AcademicTerm> AcademicTerms { get; set; } = new List<AcademicTerm>();
}
