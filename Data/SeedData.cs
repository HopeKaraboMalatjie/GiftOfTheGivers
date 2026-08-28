using GiftOfTheGivers.Models;
using Microsoft.AspNetCore.Identity;

namespace GiftOfTheGivers.Data
{
    // Seeds the two prototype roles (Employee, Donor), a demo employee login,
    // and a couple of sample relief projects so the prototype isn't empty on first run.
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var db = services.GetRequiredService<ApplicationDbContext>();

            string[] roles = { "Employee", "Donor" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            const string demoEmployeeEmail = "employee@giftofthegivers.org";
            if (await userManager.FindByEmailAsync(demoEmployeeEmail) is null)
            {
                var employee = new ApplicationUser
                {
                    UserName = demoEmployeeEmail,
                    Email = demoEmployeeEmail,
                    FullName = "Demo Employee",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(employee, "Employee@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(employee, "Employee");
                }
            }

            if (!db.ReliefProjects.Any())
            {
                db.ReliefProjects.AddRange(
                    new ReliefProject
                    {
                        Title = "Flood Relief - KwaZulu-Natal",
                        Description = "Distributing emergency food, water and shelter to communities affected by recent flooding.",
                        Location = "KwaZulu-Natal, South Africa",
                        Status = "Active"
                    },
                    new ReliefProject
                    {
                        Title = "Drought Response - Eastern Cape",
                        Description = "Coordinating water tankers and borehole repairs for drought-stricken rural areas.",
                        Location = "Eastern Cape, South Africa",
                        Status = "Active"
                    },
                     new ReliefProject
                     {
                         Title = "Earthquake Response - Mpumalanga",
                         Description = "Coordinating roads and houses repairs for stricken rural areas.",
                         Location = "Mpumalanga, South Africa",
                         Status = "Active"
                     }
                );
                await db.SaveChangesAsync();
            }
        }
    }
}
