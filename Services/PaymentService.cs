using api.Data;
using api.DTOs;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class PaymentService(ApplicationDBContext context) : IPaymentService
{
    public async Task<ApiResponse<object>> GenerateMonthlyInvoicesAsync(
        Guid tenantId,
        Guid teacherId,
        string? monthKey = null
    )
    {
        monthKey ??= DateTimeOffset.UtcNow.ToString("yyyy-MM");

        var students = await context
            .StudentGroups.Where(sg =>
                sg.Group.TenantId == tenantId
                && sg.Group.TeacherId == teacherId
                && sg.Group.PaymentType == PaymentType.Monthly
                && !sg.Student.Invoices.Any(i => i.GroupId == sg.GroupId && i.MonthKey == monthKey)
            )
            .Select(sg => new
            {
                sg.StudentId,
                sg.GroupId,
                Amount = sg.CustomPrice ?? sg.Group.Price,
            })
            .ToListAsync();

        if (students.Count == 0)
        {
            return ApiResponse<object>.Ok(new { });
        }

        var studentInvoices = students
            .Select(s => new StudentInvoice
            {
                Type = InvoiceType.Monthly,
                TotalAmount = s.Amount,
                PaidAmount = 0,
                Status = InvoiceStatus.Unpaid,
                MonthKey = monthKey,
                StudentId = s.StudentId,
                GroupId = s.GroupId,
            })
            .ToList();

        await context.StudentInvoices.AddRangeAsync(studentInvoices);
        await context.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { });
    }

    public Task<ApiResponse<PaymentsSheetDto>> GetPaymentSheetAsync(
        Guid tenantId,
        Guid teacherId,
        PaymentFilterDto filterDto
    )
    {
        throw new NotImplementedException();
    }
}
