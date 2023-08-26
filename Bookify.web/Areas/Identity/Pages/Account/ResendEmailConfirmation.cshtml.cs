// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Bookify.web.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Bookify.web.Services;

namespace Bookify.web.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IEmailBodyBuilder _emailBodyBuilder;

		public ResendEmailConfirmationModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IEmailBodyBuilder emailBodyBuilder = null)
		{
			_userManager = userManager;
			_emailSender = emailSender;
			
			_emailBodyBuilder = emailBodyBuilder;
		}

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
      
            public string Username { get; set; }
        }

        public void OnGet(string username)
        {
            Input = new InputModel()
            {
                Username =username
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

			var user = _userManager.Users.
					SingleOrDefault(u => (u.NormalizedUserName == Input.Username.ToUpper() || u.NormalizedEmail == Input.Username.ToUpper()) && !u.IsDeleted);
			if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
			protocol: Request.Scheme);


           string body= _emailBodyBuilder.EmailBody
                ("https://res.cloudinary.com/macto/image/upload/v1692517959/Confirmed-bro_c3fz1e.png"
                ,$"Hey {user.FullName}, thanks for joining us"
                ,"Please Confirm your email"
                ,$"{HtmlEncoder.Default.Encode(callbackUrl)}"
                ,"Active account");
			//var filePath = $"{_webHostEnvironment.WebRootPath}/assats/templates/email.html";
			//StreamReader reader = new StreamReader(filePath);
			//var body = reader.ReadToEnd();
			//reader.Close();
			//body = body
			//	  .Replace("[imageUrl]", )
			//	  .Replace("[header]", )
			//	  .Replace("[body]", )
			//	  .Replace("[url]", )
			//	  .Replace("[linkTitle]", );

			await _emailSender.SendEmailAsync(
                user.Email,
                "Confirm your email",body);

            ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            return Page();
        }
    }
}
