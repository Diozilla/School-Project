using FS02P2.Data.Enums;
using FS02P2.Models;
using Microsoft.AspNetCore.Identity;

namespace FS02P2.Data
{
    public class ApplicationDbContextSeed
    {
        public async static Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
          await  roleManager.CreateAsync(new IdentityRole(RolesEnum.SuperAdmin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(RolesEnum.Admin.ToString()));
            await roleManager.CreateAsync(new IdentityRole(RolesEnum.RegularUser.ToString()));

        }

        public async static Task SeedSuperAdminAsync(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager)
        {
            var superAdminUser = new ApplicationUser
            {
                UserName = "bobo@info.com",
                Email = "bobo@info.com",
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
              
                FirstName = "bo",
                LastName = "bo"



            };

            if (userManager.Users.All(u=>u.Id != superAdminUser.Id)) 
            {
                var user = await userManager.FindByEmailAsync(superAdminUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(superAdminUser, "A_123a");
                    await userManager.AddToRoleAsync(superAdminUser, RolesEnum.SuperAdmin.ToString());
                    await userManager.AddToRoleAsync(superAdminUser, RolesEnum.Admin.ToString());
                    await userManager.AddToRoleAsync(superAdminUser, RolesEnum.RegularUser.ToString());

                }
            }

            
        }
    }
}
