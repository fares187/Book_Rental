
using AutoMapper;
using Bookify.web.Core.Const;
using Bookify.web.Core.Models;
using Bookify.web.Core.ViewModel;
using Bookify.web.Filter;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Npgsql.Replication.PgOutput.Messages;
using NuGet.Common;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Bookify.web.Services;

namespace Bookify.web.Controllers
{
    [Authorize(Roles = AppRoles.Admin)]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManger;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailBodyBuilder _emailBodyBuilder;
        private readonly RoleManager<IdentityRole> _roleManager;

		public UsersController(UserManager<ApplicationUser> userManger, IMapper mapper, RoleManager<IdentityRole> roleManager = null, IEmailSender emailSender = null, IWebHostEnvironment webHostEnvironment = null, IEmailBodyBuilder emailBodyBuilder = null)
		{
			_userManger = userManger;
			_mapper = mapper;
			_emailSender = emailSender;
			_roleManager = roleManager;
			_webHostEnvironment = webHostEnvironment;
			_emailBodyBuilder = emailBodyBuilder;
		}

		public async Task<IActionResult> Index()
        {
            IEnumerable<ApplicationUser> users = await _userManger.Users.ToListAsync();
            IEnumerable<UserViewModel> ViewModel = _mapper.Map<IEnumerable<UserViewModel>>(users);
            return View(ViewModel);
        }
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Create()
        {
            var Roles = await _roleManager.Roles.ToListAsync();
            var viewModel = new UserFormViewModel()
            {
                Roles = Roles.Select(b => new SelectListItem()
                {
                    Text = b.Name,
                    Value = b.Name
                })
            };
            return PartialView("_form", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest();
            ApplicationUser user = new()
            {
                FullName = model.fullname,
                UserName = model.UserName,
                Email = model.Email,
                CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value

            };
            var result = await _userManger.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
              
                await _userManger.AddToRolesAsync(user, model.SelectedRoles);

          
                // var url = Url.Action("Index","Users",null,Request.Scheme); 
                

                var code = await _userManger.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = user.Id, code = code },
                    protocol: Request.Scheme);

              
			string body = _emailBodyBuilder.EmailBody
			  ("https://res.cloudinary.com/macto/image/upload/v1692517959/Confirmed-bro_c3fz1e.png"
			  , $"Hey {user.FullName}, thanks for joining us"
			  , "Please Confirm your email"
			  , $"{HtmlEncoder.Default.Encode(callbackUrl)}"
			  , "Active account");

				await _emailSender.SendEmailAsync(user.Email, "Confirm your email",body);


                var viewmodel = _mapper.Map<UserViewModel>(user);
                return PartialView("_UserRow", viewmodel);

            }
            return BadRequest(string.Join(',', result.Errors.Select(e => e.Description)));
        }
        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Edit(string id)
        {
            ApplicationUser user = await _userManger.FindByIdAsync(id);
            if (user is null)
                return NotFound();
            var allroles = await _roleManager.Roles.ToListAsync();
            var roles = await _userManger.GetRolesAsync(user);

            UserFormViewModel ViewModel = new()
            {
                Id = user.Id,
                Roles = allroles.Select(b => new SelectListItem()
                {
                    Value = b.Name,
                    Text = b.Name
                }),
                SelectedRoles = roles,
                UserName = user.UserName,
                fullname = user.FullName,
                Email = user.Email
            };
            return PartialView("_form", ViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserFormViewModel model)
        {

            ModelState.Remove("Password");
            ModelState.Remove("ConfirmPassword");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var user = await _userManger.FindByIdAsync(model.Id);
            if (user is null)
                return NotFound();
            user.UserName = model.UserName;
            user.FullName = model.fullname;
            user.LastUpdatedOn = DateTime.UtcNow;
            user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            user.Email = model.Email;


            var result = await _userManger.UpdateAsync(user);
            if (result.Succeeded)
            {
                var currentRoles = await _userManger.GetRolesAsync(user);
                var UpdateRoles = currentRoles.SequenceEqual(model.SelectedRoles);
                if (!UpdateRoles)
                {
                    await _userManger.RemoveFromRolesAsync(user, currentRoles);
                    await _userManger.AddToRolesAsync(user, model.SelectedRoles);

                }
                await _userManger.UpdateSecurityStampAsync(user);
                var viewmodel = _mapper.Map<UserViewModel>(user);
                return PartialView("_UserRow", viewmodel);
            }

            return BadRequest(string.Join(',', result.Errors.Select(e => e.Description)));

        }

        [HttpGet]
        [AjaxOnly]
        public async Task<IActionResult> Password(string id)
        {
            var user = await _userManger.FindByIdAsync(id);
            if (user is null)
                return NotFound();
            var viewModel = new UserPasswordViewModel()
            {
                Id = id
            };
            return PartialView("_passwordForm", id);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Password(UserPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var user = await _userManger.FindByIdAsync(model.Id);
            if (user is null)
                return NotFound();
            var token = await _userManger.GeneratePasswordResetTokenAsync(user);
            var result = await _userManger.ResetPasswordAsync(user, token, model.Password);
            if (result.Succeeded)
            {
                user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                user.LastUpdatedOn = DateTime.UtcNow;
                await _userManger.UpdateAsync(user);
                var viewModel = _mapper.Map<UserViewModel>(user);
                return PartialView("_UserRow", viewModel);
            }
            else
            {
                return BadRequest(string.Join(',', result.Errors.Select(e => e.Description)));
            }

        }
        public async Task<IActionResult> UniqeEmail(UserFormViewModel model)
        {
            var user = await _userManger.FindByEmailAsync(model.Email);
            bool go = user is null || user.Id.Equals(user.Id);

            return Json(go);
        }
        public async Task<IActionResult> UniqeUsername(UserFormViewModel model)
        {
            var user = await _userManger.FindByNameAsync(model.UserName);
            bool go = user is null || user.Id.Equals(user.Id);

            return Json(go);
        }
        public async Task<IActionResult> ToggleStatus(string id)
        {
            ApplicationUser? user = await _userManger.FindByIdAsync(id);

            if (user is null)
            {
                Debug.WriteLine("there------------------------>");
                return NotFound();
            }

            user.IsDeleted = !user.IsDeleted;
            user.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            user.LastUpdatedOn = DateTime.UtcNow;
            var result = await _userManger.UpdateAsync(user);
            if (result.Succeeded)
            {
                if (user.IsDeleted)
                    await _userManger.UpdateSecurityStampAsync(user);
                return Ok(user!.LastUpdatedOn.Value.ToString("dd MMM, yyyy"));
            }
            else
            {
                if (user.IsDeleted)
                    await _userManger.UpdateSecurityStampAsync(user);
                return BadRequest();
            }
        }
        public async Task<IActionResult> UnLock(string id)
        {
            var user = await _userManger.FindByIdAsync(id);
            if (user is null)
                return NotFound();
            var isLockedout = await _userManger.IsLockedOutAsync(user);
            if (isLockedout)
                await _userManger.SetLockoutEndDateAsync(user, null);

            return Ok();


        }
    }
}
