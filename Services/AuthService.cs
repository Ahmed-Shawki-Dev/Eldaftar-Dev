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
    RoleManager<IdentityRole<Guid>> roleManager,
    ITokenService tokenService
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

    public async Task<ApiResponse<LoginResponseDto>> LoginAsync(string slug, LoginDto dto)
    {
        var normalizedSlug = slug.ToLower().Trim();
        var compositeUserName = $"{normalizedSlug}_{dto.PhoneNumber.Trim()}";

        var user = await userManager.FindByNameAsync(compositeUserName);
        if (user == null)
        {
            return ApiResponse<LoginResponseDto>.Fail("رقم الهاتف أو كلمة المرور غير صحيحة.");
        }

        var isPasswordValid = await userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordValid)
        {
            return ApiResponse<LoginResponseDto>.Fail("رقم الهاتف أو كلمة المرور غير صحيحة.");
        }

        var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.Id == user.TenantId);
        if (tenant == null)
        {
            return ApiResponse<LoginResponseDto>.Fail("السنتر المرتبط بهذا الحساب غير موجود.");
        }

        var token = await tokenService.CreateTokenAsync(user, tenant);
        var res = new LoginResponseDto(token);

        return ApiResponse<LoginResponseDto>.Ok(res, "تم تسجيل الدخول بنجاح.");
    }
}
