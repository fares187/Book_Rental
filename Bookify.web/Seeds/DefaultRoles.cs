using Bookify.web.Core.Const;
using Microsoft.AspNetCore.Identity;

namespace Bookify.web.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedRolesAsync( RoleManager<IdentityRole> rolesManager)
        {
            if(! rolesManager.Roles.Any()) {
                await rolesManager.CreateAsync(new IdentityRole()
                {
                    Name = AppRoles.Admin,
                    
                });
                await rolesManager.CreateAsync(new IdentityRole()
                {
                    Name = AppRoles.Archive,

                });
                await rolesManager.CreateAsync(new IdentityRole()
                {
                    Name = AppRoles.Reception,

                });

            }
        }
    }
}
