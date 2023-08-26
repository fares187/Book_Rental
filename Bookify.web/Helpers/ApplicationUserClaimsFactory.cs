using Bookify.web.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Bookify.web.Helpers
{
	public class ApplicationUserClaimsFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
	{
		public ApplicationUserClaimsFactory(UserManager<ApplicationUser> userManager
			, RoleManager<IdentityRole> roleManager,
			IOptions<IdentityOptions> options) : base(userManager, roleManager, options)
		{
		}
		protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
		{
			var identity=await base.GenerateClaimsAsync(user);
			identity.AddClaim(new Claim(ClaimTypes.GivenName,user.FullName));
			return identity;	
		}
	}
}
