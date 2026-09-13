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

    public async Task<ApiResponse<PaymentsSheetDto>> GetPaymentSheetAsync(
        Guid tenantId,
        Guid teacherId,
        Guid groupId,
        PaymentFilterDto filterDto
    )
    {
        // 1. Get Group
        var group = await context
            .Groups.AsNoTracking()
            .Where(g => g.Id == groupId && g.TeacherId == teacherId && g.TenantId == tenantId)
            .Select(g => new
            {
                g.Id,
                g.Name,
                g.Grade,
                g.PaymentType,
                g.Price,
            })
            .FirstOrDefaultAsync();

        if (group == null)
        {
            return ApiResponse<PaymentsSheetDto>.Fail(
                "المجموعة غير موجودة أو غير مصرح لك بالوصول إليها"
            );
        }

        var groupName = $"{group.Grade}-{group.Name}";

        // 2. Define Sheets
        List<MonthlyStudentPaymentRowDto>? monthlySheet = null;
        List<PerSessionDebtRowDto>? sessionDebts = null;
        int totalCount = 0;
        int paidCount = 0;
        int unpaidCount = 0;

        // 3. Monthly Sheet Logic
        if (group.PaymentType == PaymentType.Monthly)
        {
            // Generate Month Invoices
            var monthKey = filterDto.MonthKey;

            // Get All Enrollments And There Month Invoice
            monthlySheet = await context
                .StudentGroups.AsNoTracking()
                .Where(sg => sg.GroupId == groupId)
                .Select(sg => new
                {
                    Invoice = sg.Student.Invoices.FirstOrDefault(i =>
                        i.GroupId == groupId && i.MonthKey == monthKey
                    ),
                    Group = sg,
                })
                .Select(x => new MonthlyStudentPaymentRowDto(
                    x.Group.StudentId,
                    x.Group.Student.Name,
                    x.Group.Student.StudentCode,
                    x.Group.Student.ParentPhone,
                    x.Invoice != null ? x.Invoice.Id : Guid.Empty,
                    x.Invoice != null
                        ? x.Invoice.TotalAmount
                        : (x.Group.CustomPrice ?? x.Group.Group.Price),
                    x.Invoice != null && x.Invoice.Status == InvoiceStatus.Paid
                ))
                .ToListAsync();

            totalCount = monthlySheet.Count;
            paidCount = monthlySheet.Count(x => x.IsPaid);
            unpaidCount = totalCount - paidCount;
        }

        // 4. PerSession Dept Logic
        if (group.PaymentType == PaymentType.PerSession)
        {
            sessionDebts = await context
                .StudentInvoices.AsNoTracking()
                .Where(si => si.GroupId == groupId && si.Status != InvoiceStatus.Paid)
                .Select(si => new PerSessionDebtRowDto(
                    si.Id,
                    si.StudentId,
                    si.Student.Name,
                    si.Student.StudentCode,
                    si.Student.ParentPhone,
                    si.SessionId!.Value,
                    si.Session!.CreatedAt,
                    si.TotalAmount - si.PaidAmount
                ))
                .ToListAsync();

            totalCount = sessionDebts.Count;
            unpaidCount = sessionDebts.Count;
            paidCount = 0;
        }

        var paymentSheet = new PaymentsSheetDto(
            group.Id,
            groupName,
            group.PaymentType,
            group.Price,
            totalCount,
            paidCount,
            unpaidCount,
            monthlySheet,
            sessionDebts
        );

        return ApiResponse<PaymentsSheetDto>.Ok(paymentSheet, "تم إرجاع شيت المستحقات بنجاح");
    }

    public async Task<ApiResponse<QuickStudentDebtDto>> GetStudentPendingInvoicesByCodeAsync(
        Guid tenantId,
        Guid teacherId,
        string studentCode
    )
    {
        var invoices = await context
            .StudentInvoices.AsNoTracking()
            .Where(si =>
                si.Group.TenantId == tenantId
                && si.Group.TeacherId == teacherId
                && si.Student.StudentCode == studentCode
                && si.Status != InvoiceStatus.Paid
            )
            .Select(si => new
            {
                // Student Data
                si.StudentId,
                si.Student.Name,
                si.Student.StudentCode,

                // Invoice Details
                Invoice = new PendingInvoiceDto
                {
                    InvoiceId = si.Id,
                    Type = si.Type,
                    GroupName = $"{si.Group.Grade}-{si.Group.Name}",
                    MonthKey = si.MonthKey,
                    SessionDate = si.Session != null ? si.Session.CreatedAt : null,
                    TotalAmount = si.TotalAmount,
                    PaidAmount = si.PaidAmount,
                },
            })
            .ToListAsync();

        if (invoices.Count == 0)
        {
            return ApiResponse<QuickStudentDebtDto>.Fail("لا توجد فواتير مستحقة لهذا الطالب");
        }

        var first = invoices[0];

        var result = new QuickStudentDebtDto
        {
            StudentId = first.StudentId,
            StudentName = first.Name,
            StudentCode = first.StudentCode,
            Invoices = invoices.Select(x => x.Invoice).ToList(),
            TotalDebt = invoices.Sum(x => x.Invoice.RemainingAmount),
        };

        return ApiResponse<QuickStudentDebtDto>.Ok(result, "تم إرجاع البيانات بنجاح");
    }
}
