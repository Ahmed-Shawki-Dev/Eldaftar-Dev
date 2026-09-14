using api.Models;

namespace api.DTOs;

public record PaymentsSheetDto(
    Guid GroupId,
    string GroupName,
    PaymentType PaymentType,
    decimal DefaultPrice,
    // Month Sheet Data
    int TotalCount,
    int PaidCount,
    int UnpaidCount,
    // real data
    List<MonthlyStudentPaymentRowDto>? MonthlySheet,
    List<PerSessionDebtRowDto>? SessionDebts
);

public record MonthlyStudentPaymentRowDto(
    Guid StudentId,
    string StudentName,
    string? StudentCode,
    string? ParentPhone,
    Guid InvoiceId,
    decimal Amount,
    bool IsPaid
);

public record PerSessionDebtRowDto(
    Guid InvoiceId,
    Guid StudentId,
    string StudentName,
    string? StudentCode,
    string? ParentPhone,
    Guid SessionId,
    DateTimeOffset SessionDate,
    decimal Amount
);

public record PaymentFilterDto(int? Year, int? Month)
{
    public string MonthKey =>
        $"{Year ?? DateTimeOffset.UtcNow.Year:D4}-{Month ?? DateTimeOffset.UtcNow.Month:D2}";
}

public class QuickStudentDebtDto
{
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentCode { get; set; } = string.Empty;
    public decimal TotalDebt { get; set; }
    public List<PendingInvoiceDto> Invoices { get; set; } = new();
}

public class PendingInvoiceDto
{
    public Guid InvoiceId { get; set; }
    public InvoiceType Type { get; set; }
    public string GroupName { get; set; } = string.Empty;
    public string? MonthKey { get; set; }
    public DateTimeOffset? SessionDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount => TotalAmount - PaidAmount;
}

public record CollectPaymentDto(Guid InvoiceId);

public record CancelPaymentDto(Guid InvoiceId);
