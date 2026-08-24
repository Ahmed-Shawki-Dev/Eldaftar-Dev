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
        // 1. Check If Slug Exists
        var slugExist = await context.Tenants.AnyAsync(t => t.Slug == dto.Slug);

        if (slugExist)
            return ApiResponse<TenantAuthResponseDto>.Fail("الـ Slug مستخدم من قبل.");

        await using var transaction = await context.Database.BeginTransactionAsync();

        try
        {
            // 2. Create Tenant Record
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = dto.CenterName,
                Slug = dto.Slug.ToLower().Trim(),
                Type = dto.TenantType,
            };

            context.Tenants.Add(tenant);

            // 3. Create AppUser
            var user = new AppUser
            {
                UserName = $"{tenant.Id}_{dto.PhoneNumber.Trim()}",
                PhoneNumber = dto.PhoneNumber.Trim(),
                TenantId = tenant.Id,
            };

            var createResult = await userManager.CreateAsync(user, dto.Password);

            if (!createResult.Succeeded)
            {
                await transaction.RollbackAsync();
                var errors = createResult.Errors.Select(e => e.Description).ToList();
                return ApiResponse<TenantAuthResponseDto>.Fail("فشل إنشاء الحساب", errors);
            }

            tenant.UserId = user.Id;

            // 4. Assign Single Role Based On Tenant Type
            if (dto.TenantType == TenantType.SoloTeacher)
            {
                const string teacherRole = "Teacher";

                if (!await roleManager.RoleExistsAsync(teacherRole))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(teacherRole));
                }

                await userManager.AddToRoleAsync(user, teacherRole);

                // Create Teacher Entity
                var teacher = new Teacher
                {
                    Name = dto.FullName,
                    TenantId = tenant.Id,
                    UserId = user.Id,
                };

                context.Teachers.Add(teacher);
            }
            else
            {
                const string adminRole = "CenterAdmin";

                if (!await roleManager.RoleExistsAsync(adminRole))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(adminRole));
                }

                await userManager.AddToRoleAsync(user, adminRole);
            }

            // 5. Commit Transaction
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
        var cleanPhone = dto.PhoneNumber.Trim();

        // 1. Get Tenant
        var tenant = await context.Tenants.FirstOrDefaultAsync(t => t.Slug == normalizedSlug);
        if (tenant == null)
            return ApiResponse<LoginResponseDto>.Fail("السنتر غير موجود.");

        // 2. Get Username And Check
        var compositeUserName = $"{tenant.Id}_{cleanPhone}";
        var user = await userManager.FindByNameAsync(compositeUserName);

        if (user == null || !await userManager.CheckPasswordAsync(user, dto.Password))
            return ApiResponse<LoginResponseDto>.Fail("رقم الهاتف أو كلمة المرور غير صحيحة.");

        // 3. Create And Retrieve Token
        var token = await tokenService.CreateTokenAsync(user, tenant);
        return ApiResponse<LoginResponseDto>.Ok(
            new LoginResponseDto(token),
            "تم تسجيل الدخول بنجاح."
        );
    }
}
