namespace api.Models;

public enum SubscriptionPlan
{
    Basic,
    Pro,
    Ultimate,
}

public class Subscription : BaseModel
{
    public SubscriptionPlan Plan { get; set; } = SubscriptionPlan.Basic;
    public int MaxStudents { get; set; } = 50;
    public decimal Price { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTimeOffset? ExpiresAt { get; set; }

    // * Tenant (One-to-One)
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;
}
