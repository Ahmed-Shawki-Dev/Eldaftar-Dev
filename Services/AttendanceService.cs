using api.Data;
using api.DTOs;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class AttendanceService(ApplicationDBContext context) : IAttendanceService
{
    public async Task<ApiResponse<object>> BulkAttendanceAsync(
        Guid tenantId,
        Guid teacherId,
        Guid userId,
        Guid sessionId,
        BulkAttendanceDto dto
    )
    {
        // 1. get session
        var session = await context
            .Sessions.Include(s => s.Group)
            .FirstOrDefaultAsync(s =>
                s.Id == sessionId && s.Group.TeacherId == teacherId && s.Group.TenantId == tenantId
            );

        if (session == null)
        {
            return ApiResponse<object>.Fail("الحصة غير موجودة");
        }

        // 2. get current state for attendance and invoices
        var prevAttendanceSheetState = await context
            .Attendances.Where(a => a.SessionId == sessionId)
            .ToDictionaryAsync(a => a.StudentId);

        var existingInvoices = new Dictionary<Guid, StudentInvoice>();
        var customPrices = new Dictionary<Guid, decimal?>();

        if (session.Group.PaymentType == PaymentType.PerSession)
        {
            existingInvoices = await context
                .StudentInvoices.Where(si => si.SessionId == sessionId)
                .ToDictionaryAsync(si => si.StudentId);

            customPrices = await context
                .StudentGroups.AsNoTracking()
                .Where(sg => sg.GroupId == session.GroupId)
                .ToDictionaryAsync(sg => sg.StudentId, sg => sg.CustomPrice);
        }

        // 3. loop on students
        foreach (var student in dto.Students)
        {
            // attendance
            if (prevAttendanceSheetState.TryGetValue(student.StudentId, out var existingAttendance))
            {
                existingAttendance.Status = student.Status;
            }
            else
            {
                var studentAttendance = new Attendance
                {
                    Status = student.Status,
                    IsMakeup = false,
                    SessionId = sessionId,
                    StudentId = student.StudentId,
                };
                context.Attendances.Add(studentAttendance);
            }

            // invoices
            if (session.Group.PaymentType == PaymentType.PerSession)
            {
                bool hasInvoice = existingInvoices.TryGetValue(
                    student.StudentId,
                    out var existingInvoice
                );

                // State 1: Student Present, Don't have Invoice.
                if (student.Status == AttendanceStatus.Present && !hasInvoice)
                {
                    decimal sessionPrice =
                        customPrices.GetValueOrDefault(student.StudentId) ?? session.Group.Price;

                    var invoice = new StudentInvoice
                    {
                        StudentId = student.StudentId,
                        GroupId = session.GroupId,
                        SessionId = sessionId,
                        Type = InvoiceType.PerSession,
                        TotalAmount = sessionPrice,
                        PaidAmount = student.HasPaidSession ? sessionPrice : 0,
                        Status = student.HasPaidSession ? InvoiceStatus.Paid : InvoiceStatus.Unpaid,
                    };

                    if (student.HasPaidSession)
                    {
                        invoice.Transactions.Add(
                            new PaymentTransaction
                            {
                                Amount = sessionPrice,
                                Method = PaymentMethod.Cash,
                                ReceivedByUserId = userId,
                                Note = $"تحصيل حصة: {session.Group.Name}",
                            }
                        );
                    }

                    context.StudentInvoices.Add(invoice);
                    existingInvoices[student.StudentId] = invoice;
                }
                // State 2 : Student Present, Dom't Pay But After This Paid
                else if (
                    hasInvoice
                    && existingInvoice!.Status == InvoiceStatus.Unpaid
                    && student.Status == AttendanceStatus.Present
                    && student.HasPaidSession
                )
                {
                    existingInvoice.Status = InvoiceStatus.Paid;
                    existingInvoice.PaidAmount = existingInvoice.TotalAmount;

                    context.PaymentTransactions.Add(
                        new PaymentTransaction
                        {
                            InvoiceId = existingInvoice.Id,
                            Amount = existingInvoice.TotalAmount,
                            Method = PaymentMethod.Cash,
                            ReceivedByUserId = userId,
                            Note = $"تحصيل متأخرات حصة: {session.Group.Name}",
                        }
                    );
                }
                // State 3 : Student Present By Mistake And We Made It Absent
                else if (
                    hasInvoice
                    && existingInvoice!.Status == InvoiceStatus.Unpaid
                    && student.Status == AttendanceStatus.Absent
                )
                {
                    context.StudentInvoices.Remove(existingInvoice);
                    existingInvoices.Remove(student.StudentId);
                }
                // State 4: Student was marked Paid by mistake, revert back to Unpaid
                else if (
                    hasInvoice
                    && existingInvoice!.Status == InvoiceStatus.Paid
                    && student.Status == AttendanceStatus.Present
                    && !student.HasPaidSession
                )
                {
                    existingInvoice.Status = InvoiceStatus.Unpaid;
                    existingInvoice.PaidAmount = 0;

                    var transactionsToRemove = context.PaymentTransactions.Where(pt =>
                        pt.InvoiceId == existingInvoice.Id
                    );
                    context.PaymentTransactions.RemoveRange(transactionsToRemove);
                }
            }
        }

        // 4. update session
        session.Status = SessionStatus.Completed;

        // 5. save
        await context.SaveChangesAsync();

        return ApiResponse<object>.Ok("تم تسجيل الحضور وتحديث الحسابات بنجاح");
    }

    // ------------------------------------------------------------------------------------------

    public async Task<ApiResponse<StudentAttendanceRowDto>> AddVisitorStudentToSessionAsync(
        Guid tenantId,
        Guid teacherId,
        Guid userId,
        Guid sessionId,
        AddVisitorStudentDto dto
    )
    {
        // 1. Get Session
        var session = await context
            .Sessions.Include(s => s.Group)
            .FirstOrDefaultAsync(s =>
                s.Id == sessionId && s.Group.TeacherId == teacherId && s.Group.TenantId == tenantId
            );

        if (session == null)
        {
            return ApiResponse<StudentAttendanceRowDto>.Fail("الحصة غير موجودة");
        }

        // 2. Search Student
        var visitorStudent = await context.Students.FirstOrDefaultAsync(s =>
            s.StudentCode == dto.StudentCode && s.TenantId == tenantId
        );

        if (visitorStudent == null)
        {
            return ApiResponse<StudentAttendanceRowDto>.Fail("الطالب غير موجود");
        }

        // 3. Ensure not enrolled originally in the group
        var isAlreadyEnrolled = await context.StudentGroups.AnyAsync(sg =>
            sg.GroupId == session.GroupId && sg.StudentId == visitorStudent.Id
        );

        if (isAlreadyEnrolled)
        {
            return ApiResponse<StudentAttendanceRowDto>.Fail("الطالب مقيد بالفعل في هذه المجموعة");
        }

        // 4. Ensure not already added to this session
        var isAlreadyInSession = await context.Attendances.AnyAsync(a =>
            a.SessionId == sessionId && a.StudentId == visitorStudent.Id
        );

        if (isAlreadyInSession)
        {
            return ApiResponse<StudentAttendanceRowDto>.Fail("الطالب مسجل بالفعل في شيت الحصة");
        }

        // 5. Add Attendance as Visitor
        var attendance = new Attendance
        {
            SessionId = sessionId,
            StudentId = visitorStudent.Id,
            Status = AttendanceStatus.Present,
            IsMakeup = true,
        };
        context.Attendances.Add(attendance);

        // 6. Handle Default Cash Collection for Visitor
        bool isPaid = false;
        if (session.Group.PaymentType == PaymentType.PerSession)
        {
            isPaid = true;
            var sessionPrice = session.Group.Price;

            var invoice = new StudentInvoice
            {
                StudentId = visitorStudent.Id,
                GroupId = session.GroupId,
                SessionId = sessionId,
                Type = InvoiceType.PerSession,
                TotalAmount = sessionPrice,
                PaidAmount = sessionPrice,
                Status = InvoiceStatus.Paid,
                Transactions = new List<PaymentTransaction>
                {
                    new PaymentTransaction
                    {
                        Amount = sessionPrice,
                        Method = PaymentMethod.Cash,
                        ReceivedByUserId = userId,
                        Note = $"تحصيل حصة طالب زائر: {session.Group.Name}",
                    },
                },
            };

            context.StudentInvoices.Add(invoice);
        }

        // 7. Save Changes
        await context.SaveChangesAsync();

        // 8. Return Row to Frontend
        return ApiResponse<StudentAttendanceRowDto>.Ok(
            new StudentAttendanceRowDto(
                StudentId: visitorStudent.Id,
                StudentName: visitorStudent.Name,
                StudentCode: visitorStudent.StudentCode,
                AttendanceStatus: AttendanceStatus.Present,
                HasPaidSession: isPaid,
                RequiredPrice: session.Group.PaymentType == PaymentType.PerSession
                    ? session.Group.Price
                    : null,
                IsVisitorStudent: true
            ),
            "تم تسجيل الطالب الزائر وتحصيل الحصة بنجاح."
        );
    }
}
