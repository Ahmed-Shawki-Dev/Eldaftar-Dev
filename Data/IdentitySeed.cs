using Microsoft.AspNetCore.Identity;

namespace api.Data;

public static class IdentitySeed
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole<Guid>> roleManager)
    {
        string[] roleNames = ["SuperAdmin", "CenterAdmin", "Teacher", "Assistant"];

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }
    }
}
