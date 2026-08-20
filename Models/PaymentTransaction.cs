namespace api.Models;

public enum PaymentMethod
{
    Cash = 1,
    VodafoneCash = 2,
    InstaPay = 3,
    Other = 4,
}

public class PaymentTransaction : BaseModel
{
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.Cash;
    public string? Note { get; set; }

    // Relations
    public Guid InvoiceId { get; set; }
    public StudentInvoice Invoice { get; set; } = null!;

    // 2. Received By
    public Guid ReceivedByUserId { get; set; }
    public AppUser ReceivedByUser { get; set; } = null!;
}
