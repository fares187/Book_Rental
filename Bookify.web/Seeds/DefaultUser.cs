using Bookify.web.Core.Const;
using Bookify.web.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing.Constraints;

namespace Bookify.web.Seeds
{
    public static class DefaultUser
    {
        public static async Task SeedUsersAcync(UserManager<ApplicationUser> UserManager)
        {
            if (!UserManager.Users.Any())
            {
                ApplicationUser user = new ()
                {
                    UserName="marco34",
                    Email="admim@bookify.com",
                    FullName = "marco",
                    EmailConfirmed = true,

                };
                var result =await UserManager.CreateAsync(user,"P@ssword121");
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user,AppRoles.Admin);
                }
                
    

            }
        }
    }
}
