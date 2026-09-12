using api.DTOs;

namespace api.Interfaces;

public interface IPaymentService
{
    Task<ApiResponse<object>> GenerateMonthlyInvoicesAsync(
        Guid tenantId,
        Guid teacherId,
        string monthKey
    );

    Task<ApiResponse<PaymentsSheetDto>> GetPaymentSheetAsync(
        Guid tenantId,
        Guid teacherId,
        PaymentFilterDto filterDto
    );
}
