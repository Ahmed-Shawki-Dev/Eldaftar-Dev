using api.Data;
using api.Interfaces;
using api.Models;
using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class BackupService(ApplicationDBContext context) : IBackupService
{
    public async Task<byte[]> ExportFullTeacherDataExcelAsync(Guid tenantId, Guid teacherId)
    {
        var students = await context
            .StudentGroups.AsNoTracking()
            .Where(sg => sg.Group.TeacherId == teacherId && sg.Group.TenantId == tenantId)
            .Select(sg => sg.Student)
            .Distinct()
            .ToListAsync();

        var groups = await context
            .Groups.AsNoTracking()
            .Where(g => g.TeacherId == teacherId && g.TenantId == tenantId)
            .Select(g => new
            {
                g.Id,
                g.Name,
                g.PaymentType,
                g.Price,
                StudentCount = g.StudentGroups.Count(),
            })
            .ToListAsync();

        var invoices = await context
            .StudentInvoices.AsNoTracking()
            .Where(si =>
                si.Group.TeacherId == teacherId
                && si.Group.TenantId == tenantId
                && si.Status == InvoiceStatus.Paid
            )
            .Select(si => new
            {
                si.Id,
                StudentName = si.Student.Name,
                GroupName = si.Group.Name,
                si.TotalAmount,
                si.Status,
                si.CreatedAt,
            })
            .ToListAsync();

        using var workbook = new XLWorkbook();

        var studentsSheet = workbook.Worksheets.Add("الطلاب");
        studentsSheet.RightToLeft = true;

        studentsSheet.Cell(1, 1).Value = "كود الطالب";
        studentsSheet.Cell(1, 2).Value = "اسم الطالب";
        studentsSheet.Cell(1, 3).Value = "رقم الهاتف";
        studentsSheet.Cell(1, 4).Value = "رقم ولي الأمر";

        var studentHeader = studentsSheet.Range("A1:D1");
        studentHeader.Style.Font.Bold = true;
        studentHeader.Style.Fill.BackgroundColor = XLColor.FromArgb(30, 41, 59);
        studentHeader.Style.Font.FontColor = XLColor.White;
        studentHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        for (int i = 0; i < students.Count; i++)
        {
            int row = i + 2;
            studentsSheet.Cell(row, 1).Value = students[i].StudentCode ?? "-";
            studentsSheet.Cell(row, 2).Value = students[i].Name;
            studentsSheet.Cell(row, 3).Value = students[i].Phone ?? "-";
            studentsSheet.Cell(row, 4).Value = students[i].ParentPhone ?? "-";
        }

        studentsSheet.Columns().AdjustToContents();

        var groupsSheet = workbook.Worksheets.Add("المجموعات");
        groupsSheet.RightToLeft = true;

        groupsSheet.Cell(1, 1).Value = "اسم المجموعة";
        groupsSheet.Cell(1, 2).Value = "نظام الدفع";
        groupsSheet.Cell(1, 3).Value = "السعر";
        groupsSheet.Cell(1, 4).Value = "عدد الطلاب";

        var groupsHeader = groupsSheet.Range("A1:D1");
        groupsHeader.Style.Font.Bold = true;
        groupsHeader.Style.Fill.BackgroundColor = XLColor.FromArgb(30, 41, 59);
        groupsHeader.Style.Font.FontColor = XLColor.White;
        groupsHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        for (int i = 0; i < groups.Count; i++)
        {
            int row = i + 2;
            groupsSheet.Cell(row, 1).Value = groups[i].Name;
            groupsSheet.Cell(row, 2).Value = groups[i].PaymentType.ToString();
            groupsSheet.Cell(row, 3).Value = groups[i].Price;
            groupsSheet.Cell(row, 4).Value = groups[i].StudentCount;
        }

        groupsSheet.Columns().AdjustToContents();

        var invoicesSheet = workbook.Worksheets.Add("الفواتير");
        invoicesSheet.RightToLeft = true;

        invoicesSheet.Cell(1, 1).Value = "اسم الطالب";
        invoicesSheet.Cell(1, 2).Value = "المجموعة";
        invoicesSheet.Cell(1, 3).Value = "المبلغ الإجمالي";
        invoicesSheet.Cell(1, 4).Value = "الحالة";
        invoicesSheet.Cell(1, 5).Value = "تاريخ الإنشاء";

        var invoicesHeader = invoicesSheet.Range("A1:E1");
        invoicesHeader.Style.Font.Bold = true;
        invoicesHeader.Style.Fill.BackgroundColor = XLColor.FromArgb(30, 41, 59);
        invoicesHeader.Style.Font.FontColor = XLColor.White;
        invoicesHeader.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        for (int i = 0; i < invoices.Count; i++)
        {
            int row = i + 2;
            invoicesSheet.Cell(row, 1).Value = invoices[i].StudentName;
            invoicesSheet.Cell(row, 2).Value = invoices[i].GroupName;
            invoicesSheet.Cell(row, 3).Value = invoices[i].TotalAmount;
            invoicesSheet.Cell(row, 4).Value = invoices[i].Status.ToString();
            invoicesSheet.Cell(row, 5).Value = invoices[i].CreatedAt.ToString("yyyy-MM-dd");
        }

        invoicesSheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
