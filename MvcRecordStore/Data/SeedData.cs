using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MvcRecordStore.Data;
using MvcRecordStore.Models;
using NuGet.Configuration;

namespace MvcRecordStore.Data;

public class SeedData // Seeds the database and adds two base users (one is Admin)
{
    public static async Task Initialize(IServiceProvider serviceProvider, StoreDbContext context)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<StoreUser>>();

            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var users = new[]
            {
                new { Username = "admin@admin.com", Email = "admin@admin.com", Password = "Admin@123", Role = "Admin" },
                new { Username = "testuser@email.com", Email = "testuser@email.com", Password = "Testuser@123", Role = "User" }
            };

            foreach (var userInfo in users)
            {
                if (userManager.Users.Count() == users.Count())
                {
                    break;
                }

                if (userManager.Users.Any(u => u.NormalizedEmail == userInfo.Email.ToUpper()))
                {
                    continue;
                }

                var user = new StoreUser
                {
                    UserName = userInfo.Username,
                    Email = userInfo.Email,
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, userInfo.Password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, userInfo.Role);
                }
            }

            return;
        }
    }
}
