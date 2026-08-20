namespace api.Models;

public enum InvoiceType
{
    Monthly = 1,
    PerSession = 2,
    Material = 3,
}

public enum InvoiceStatus
{
    Unpaid = 1,
    PartiallyPaid = 2,
    Paid = 3,
}

public class StudentInvoice : BaseModel
{
    public InvoiceType Type { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; } = 0;
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Unpaid;

    public string? MonthKey { get; set; }
    public string? Note { get; set; }

    // Relations
    public Guid StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public Guid GroupId { get; set; }
    public Group Group { get; set; } = null!;

    public Guid? SessionId { get; set; }
    public Session? Session { get; set; }

    // Payment Transactions
    public ICollection<PaymentTransaction> Transactions { get; set; } =
        new List<PaymentTransaction>();
}
