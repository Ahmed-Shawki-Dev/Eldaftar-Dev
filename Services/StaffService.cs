using api.Data;
using api.DTOs;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class StaffService(
    ApplicationDBContext context,
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager
) : IStaffService
{
    public async Task<ApiResponse<StaffDto>> CreateStaffAsync(
        Guid tenantId,
        Guid teacherId,
        CreateStaffDto dto
    )
    {
        var cleanPhone = dto.PhoneNumber.Trim();
        var compositeUserName = $"{tenantId}_{cleanPhone}";

        // Check If Phone Number Repeated
        var userExists = await userManager.FindByNameAsync(compositeUserName);
        if (userExists != null)
        {
            return ApiResponse<StaffDto>.Fail("رقم الهاتف مسجل بالفعل في هذا السنتر.");
        }

        // Check If Teacher Exist
        var teacherExists = await context.Teachers.AnyAsync(t =>
            t.Id == teacherId && t.TenantId == tenantId
        );
        if (!teacherExists)
        {
            return ApiResponse<StaffDto>.Fail("المدرس غير موجود.");
        }

        // Start Transaction
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var user = new AppUser
            {
                UserName = compositeUserName,
                PhoneNumber = cleanPhone,
                TenantId = tenantId,
            };

            var userResult = await userManager.CreateAsync(user, dto.Password);
            if (!userResult.Succeeded)
            {
                await transaction.RollbackAsync();
                return ApiResponse<StaffDto>.Fail(
                    "فشل إنشاء الحساب",
                    userResult.Errors.Select(e => e.Description).ToList()
                );
            }

            // Add Staff Role
            const string roleName = "Staff";
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
            await userManager.AddToRoleAsync(user, roleName);

            // Create Staff Model
            var staff = new Staff
            {
                Name = dto.Name.Trim(),
                TeacherId = teacherId,
                UserId = user.Id,
            };

            context.Staffs.Add(staff);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            var resultDto = new StaffDto(
                staff.Id,
                staff.Name,
                user.PhoneNumber!,
                staff.TeacherId,
                staff.CreatedAt
            );
            return ApiResponse<StaffDto>.Ok(resultDto, "تم إضافة السكرتيرة بنجاح.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponse<StaffDto>.Fail($"حدث خطأ غير متوقع: {ex.Message}");
        }
    }

    public async Task<ApiResponse<List<StaffDto>>> GetAllStaffAsync(Guid tenantId, Guid teacherId)
    {
        var staffList = await context
            .Staffs.AsNoTracking()
            .Where(s => s.TeacherId == teacherId && s.Teacher.TenantId == tenantId)
            .Include(s => s.User)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new StaffDto(
                s.Id,
                s.Name,
                s.User.PhoneNumber ?? string.Empty,
                s.TeacherId,
                s.CreatedAt
            ))
            .ToListAsync();
        return ApiResponse<List<StaffDto>>.Ok(staffList, "تم جلب بيانات السكرتيرة بنجاح.");
    }

    public async Task<ApiResponse<StaffDto>> UpdateStaffAsync(
        Guid tenantId,
        Guid teacherId,
        Guid staffId,
        UpdateStaffDto dto
    )
    {
        // Fetch Staff With User
        var staff = await context
            .Staffs.Include(s => s.User)
            .FirstOrDefaultAsync(s =>
                s.Id == staffId && s.TeacherId == teacherId && s.Teacher.TenantId == tenantId
            );

        if (staff == null)
        {
            return ApiResponse<StaffDto>.Fail("السكرتيرة غير موجودة أو لا تتبع هذا المدرس.");
        }

        var cleanPhone = dto.PhoneNumber.Trim();
        var compositeUserName = $"{tenantId}_{cleanPhone}";

        // Check Phone Conflict If Phone Number Changed
        if (staff.User.PhoneNumber != cleanPhone)
        {
            var userExists = await userManager.FindByNameAsync(compositeUserName);
            if (userExists != null && userExists.Id != staff.UserId)
            {
                return ApiResponse<StaffDto>.Fail("رقم الهاتف مسجل بالفعل لسكرتيرة/مستخدم آخر.");
            }

            staff.User.PhoneNumber = cleanPhone;
            staff.User.UserName = compositeUserName;
        }

        // Reset Password If Provided
        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(staff.User);
            var resetResult = await userManager.ResetPasswordAsync(
                staff.User,
                token,
                dto.NewPassword
            );

            if (!resetResult.Succeeded)
            {
                return ApiResponse<StaffDto>.Fail(
                    "فشل تحديث كلمة المرور",
                    resetResult.Errors.Select(e => e.Description).ToList()
                );
            }
        }

        // Update Staff Entity
        staff.Name = dto.Name.Trim();
        await context.SaveChangesAsync();

        var resultDto = new StaffDto(
            staff.Id,
            staff.Name,
            staff.User.PhoneNumber!,
            staff.TeacherId,
            staff.CreatedAt
        );

        return ApiResponse<StaffDto>.Ok(resultDto, "تم تحديث بيانات السكرتيرة بنجاح.");
    }

    public async Task<ApiResponse<object>> DeleteStaffAsync(
        Guid tenantId,
        Guid teacherId,
        Guid staffId
    )
    {
        var staff = await context
            .Staffs.Include(s => s.User)
            .FirstOrDefaultAsync(s =>
                s.Id == staffId && s.TeacherId == teacherId && s.Teacher.TenantId == tenantId
            );

        if (staff == null)
        {
            return ApiResponse<object>.Fail("السكرتيرة غير موجودة أو لا تتبع هذا المدرس.");
        }

        // Delete User From AspNetUsers
        await userManager.DeleteAsync(staff.User);

        // Delete Staff Entity
        context.Staffs.Remove(staff);

        await context.SaveChangesAsync();

        return ApiResponse<object>.Ok(new { }, "تم حذف السكرتيرة بنجاح.");
    }
}
