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
    bool IsPaid,
    DateTimeOffset? PaidAt
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
