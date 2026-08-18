using api.Data;
using api.DTOs;
using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class AuthService(
    ApplicationDBContext context,
    UserManager<AppUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager
) : IAuthService
{
    public async Task<ApiResponse<TenantAuthResponseDto>> RegisterTenantAsync(RegisterTenantDto dto)
    {
        var slugExist = await context.Tenants.AnyAsync(t => t.Slug == dto.Slug);
        if (slugExist)
            return ApiResponse<TenantAuthResponseDto>.Fail("ال Slug مستخدم من قبل.");

        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = dto.CenterName,
                Slug = dto.Slug,
                Type = dto.TenantType,
            };
            context.Tenants.Add(tenant);

            var user = new AppUser
            {
                UserName = $"{tenant.Slug}_{dto.PhoneNumber}",
                PhoneNumber = dto.PhoneNumber,
                TenantId = tenant.Id,
            };

            var createResult = await userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                await transaction.RollbackAsync();
                var errors = createResult.Errors.Select(e => e.Description).ToList();
                return ApiResponse<TenantAuthResponseDto>.Fail("فشل إنشاء الحساب", errors);
            }

            const string adminRole = "CenterAdmin";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(adminRole));
            }
            await userManager.AddToRoleAsync(user, adminRole);
            tenant.UserId = user.Id;

            if (dto.TenantType == TenantType.SoloTeacher)
            {
                const string teacherRole = "Teacher";
                if (!await roleManager.RoleExistsAsync(teacherRole))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(teacherRole));
                }
                await userManager.AddToRoleAsync(user, teacherRole);

                var teacher = new Teacher
                {
                    Name = dto.FullName,
                    TenantId = tenant.Id,
                    UserId = user.Id,
                };
                context.Teachers.Add(teacher);
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            var response = new TenantAuthResponseDto(
                tenant.Id,
                tenant.Name,
                tenant.Slug,
                user.Id,
                user.PhoneNumber!,
                dto.FullName
            );

            return ApiResponse<TenantAuthResponseDto>.Ok(response, "تم إنشاء الحساب بنجاح.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return ApiResponse<TenantAuthResponseDto>.Fail($"حدث خطأ غير متوقع: {ex.Message}");
        }
    }
}
