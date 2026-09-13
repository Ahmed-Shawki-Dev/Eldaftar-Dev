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
        Guid groupId,
        PaymentFilterDto filterDto
    );

    Task<ApiResponse<QuickStudentDebtDto>> GetStudentPendingInvoicesByCodeAsync(
        Guid tenantId,
        Guid teacherId,
        string studentCode
    );

    Task<ApiResponse<object>> CollectPaymentAsync(
        Guid tenantId,
        Guid teacherId,
        Guid userId,
        CollectPaymentDto dto
    );
}
