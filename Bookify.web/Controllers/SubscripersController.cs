using AutoMapper;
using Bookify.web.Core.Const;
using Bookify.web.Core.Models;
using Bookify.web.Core.ViewModel;
using Bookify.web.Data;

using Bookify.web.Filter;
using Bookify.web.Migrations;
using Bookify.web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Bookify.web.Controllers
{
    [Authorize(Roles = AppRoles.Reception)]
    public class SubscripersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IDataProtector _dataProtector; 
        private readonly IMapper _mapper;
        private readonly IEmailBodyBuilder _emailBodyBuilder;
        private readonly IEmailSender _emailSender;
        private readonly IImageService _imageService;
        private readonly IBackgroundTaskQueue _queue;
        private readonly List<string> _allowedExtentions = new()
        {
            ".jpg",".jpeg",".png"
        };
        private readonly int _maxAllowedSize = 2097152;
        public SubscripersController(ApplicationDbContext context, IMapper mapper, IImageService imageService, IDataProtectionProvider dataProtectionProvider, IEmailSender emailSender = null, IEmailBodyBuilder emailBodyBuilder = null, IBackgroundTaskQueue queue = null)
        {
            _context = context;
            _mapper = mapper;
            _imageService = imageService;
            _dataProtector = dataProtectionProvider.CreateProtector("MySecureKey");
            _emailSender = emailSender;
            _emailBodyBuilder = emailBodyBuilder;
            _queue = queue;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Create()
        {

            return View("Form", ReturnViewModels());

        }
        [HttpPost]
        public async Task<IActionResult> Create(SubscriperFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", ReturnViewModels(model));
            if (model.Image is not null)
            {
                string extesion = model.Image.FileName;
                if (!_allowedExtentions.Contains(Path.GetExtension(extesion)))
                {
                    ModelState.AddModelError(nameof(model.Image), Core.Const.Error.extensition);
                    return View("Form", ReturnViewModels(model));
                }
                if (model.Image.Length > _maxAllowedSize)
                {
                    ModelState.AddModelError(nameof(model.Image), Core.Const.Error.maxsize);
                    return View("Form", ReturnViewModels(model));
                }
            }
            string ImageName = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
            var result = await _imageService.UploadAsync(model.Image, ImageName, "/Images/Subscribers", true);
            if (!result.IsUploaded)
            {
                ModelState.AddModelError("image", result.ErrorMassage);
                return View("Create", ReturnViewModels(model));
            }


            var sub = new Subscriper()
            {
                Address = model.Address,
                AreaId = model.AreaId,
                CreatedOn = DateTime.UtcNow,
                CreatedById = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                dateOfBirth = model.dateOfBirth,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                GovernorateId = model.GovernorateId,
                HasWhatApp = model.HasWhatApp,
                ImageUrl = $"/Images/Subscribers/{ImageName}",
                ThumbnailUrl = $"/Images/Subscribers/thumb/{ImageName}",
                IsBlackListed = false,
                MobileNumber = model.MobileNumber,
                NationalId = model.NationalId,

            };

            Subscription subscription = new()
            {
                CreatedById=sub.CreatedById,
                CreatedOn=sub.CreatedOn,
                StartDate=DateTime.Today,
                EndDate=DateTime.Today.AddYears(1)
            };
            sub.Subscriptions.Add(subscription);    
            _context.Add(sub);
            _context.SaveChanges();
            var subId = _dataProtector.Protect(sub.Id.ToString());
            return RedirectToAction("Details", new {Id= subId });

        }
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var subscriberId = int.Parse(_dataProtector.Unprotect(id));
            var user = _context.Subscripers.FirstOrDefault(b => b.Id == subscriberId);
            if (user is null)
                return NotFound();
            var ViewModel = _mapper.Map<SubscriperFormViewModel>(user);
            ViewModel.key = id;
            return View("Form", ReturnViewModels(ViewModel));

        }
        [HttpPost]
        public async Task<IActionResult> Edit(SubscriperFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var subscriberId = int.Parse(_dataProtector.Unprotect(model.key));
            Subscriper? subscriper = await _context.Subscripers.FirstOrDefaultAsync(b => b.Id == subscriberId);
            if (subscriper is null)
                return NotFound();
            _mapper.Map(model, subscriper);

            if (model.Image is not null)
            {
                string Imagename = $"{Guid.NewGuid()}{Path.GetExtension(model.Image.FileName)}";
                _imageService.Delete(model.ImageUrl, model.ThumbnailUrl);
                var result = await _imageService.UploadAsync(model.Image, Imagename, "/Images/Subscribers", true);
                if (!result.IsUploaded)
                {
                    ModelState.AddModelError(model.ImageUrl, result.ErrorMassage);
                }
                subscriper.ImageUrl = $"/Images/Subscribers/{Imagename}";
                subscriper.ThumbnailUrl = $"/Images/Subscribers/thumb/{Imagename}";
                model.ImageUrl = $"/Image/Subscribers/{Imagename}";
                model.ThumbnailUrl = $"/Image/Subscribers/thumb/{Imagename}";

            }
            subscriper.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            subscriper.LastUpdatedOn = DateTime.UtcNow;
            _context.Update(subscriper);
            _context.SaveChanges();

            return RedirectToAction("Details", new {Id=model.key});
        }

        //[AjaxOnly]
        public IActionResult GetAreas(int id)
        {
            var areas = _context.Areas.Where(b => b.GovernorateId == id).Select(d =>
                new SelectListItem()
                {
                    Value = d.Id.ToString(),
                    Text = d.Name,
                }
                ).OrderBy(b => b.Text).ToList();

            return Ok(areas);
        }
        [HttpGet]
        public IActionResult Details(string id)
        {
            var subscriberId = int.Parse(_dataProtector.Unprotect(id)) ;

            var sub = _context.Subscripers.Include(b=>b.Governorate)
                .Include(b=>b.Area)
                .Include(b=>b.Governorate)
                .Include(b=>b.Subscriptions)
                .Include(b=>b.Rentals)
                .ThenInclude(b=>b.RentalCopies)
                .SingleOrDefault(s=>s.Id== subscriberId);
            if (sub is null)
                return NotFound();
            var viewModel = _mapper.Map<SubscriperDetailsViewModel>(sub);
            viewModel.Key = id;
            return View("Details",viewModel);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Search(SearchFormViewModel model)
        {
            if (!ModelState.IsValid) 
                return View("Index",model);
            var subscriper = _context.Subscripers
                .SingleOrDefault(
                b=>b.Email==model.Value || 
                            b.NationalId== model.Value|| 
                            b.MobileNumber== model.Value);

            var viewModel = _mapper.Map<SubscriberSearchViewModel>(subscriper);
            if(subscriper is not null)
            viewModel.Key = _dataProtector.Protect(subscriper.Id.ToString());
            return PartialView("_Result",viewModel);
        }
        public IActionResult UniqeNationalId(SubscriperFormViewModel model)
        {
            var ModelId = 0;
            if(!string.IsNullOrEmpty(model.key) )
                ModelId = int.Parse(_dataProtector.Unprotect(model.key));
            if (ModelId > 0)
            {
                bool iswith = _context.Subscripers.Where(c => c.Id != ModelId).Any(c => c.NationalId == model.NationalId);
                return Json(!iswith);
            }
            bool IsAcategory = _context.Subscripers.Any(c => c.NationalId == model.NationalId);
            return Json(!IsAcategory);
        }
        public IActionResult UniqeEmail(SubscriperFormViewModel model)
        {
            var ModelId = 0;
            if (!string.IsNullOrEmpty(model.key))
                ModelId = int.Parse(_dataProtector.Unprotect(model.key));
            if (ModelId > 0)
            {
                bool iswith = _context.Subscripers.Where(c => c.Id != ModelId).Any(c => c.Email == model.Email);
                return Json(!iswith);
            }
            bool IsAcategory = _context.Subscripers.Any(c => c.Email == model.Email);
            return Json(!IsAcategory);
        }
        public IActionResult UniqeMobileNumber(SubscriperFormViewModel model)
        {
            var ModelId = 0;
            if (!string.IsNullOrEmpty(model.key))
                ModelId = int.Parse(_dataProtector.Unprotect(model.key));
            if (ModelId > 0)
            {
                bool iswith = _context.Subscripers.Where(c => c.Id != ModelId).Any(c => c.MobileNumber == model.MobileNumber);
                return Json(!iswith);
            }
            bool IsAcategory = _context.Subscripers.Any(c => c.MobileNumber == model.MobileNumber);
            return Json(!IsAcategory);
        }


        public SubscriperFormViewModel ReturnViewModels(SubscriperFormViewModel? model = null)
        {
            SubscriperFormViewModel ViewModel = (model is null) ? new SubscriperFormViewModel() : model;
            var Governorates = _context.Governorates.Where(b => !b.IsDeleted).OrderBy(b => b.Name).ToList();
            //  var Area = _context.Governorates.Where(b => !b.IsDeleted).OrderBy(b => b.Name).ToList();
            if (model is not null)
            {
                var Area = _context.Areas.Where(b => !b.IsDeleted && b.GovernorateId == model.GovernorateId).OrderBy(b => b.Name).ToList();
                ViewModel.Areas = _mapper.Map<List<SelectListItem>>(Area);
            }

            ViewModel.Governorates = _mapper.Map<List<SelectListItem>>(Governorates);

            return ViewModel;

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RenewSubscription(string key)
        {
            
            var subscriperId = int.Parse(_dataProtector.Unprotect(key));
            var subscriber = _context.Subscripers.Include(b => b.Subscriptions).SingleOrDefault(b => b.Id == subscriperId);
            if (subscriber is null)
                return NotFound();
            if (subscriber.IsBlackListed)
                return BadRequest();
            var lastsubscription = subscriber.Subscriptions.Last();
            var startDate= (lastsubscription.EndDate< DateTime.Today) ? DateTime.Today : lastsubscription.EndDate.AddDays(1);  
            Subscription subscription = new()
            {
                CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value,
                CreatedOn= DateTime.UtcNow,  
                StartDate=startDate,
                EndDate=startDate.AddYears(1)
            };
            
            subscriber.Subscriptions.Add(subscription);

           
            // ToDo: send email
            string body = _emailBodyBuilder.EmailBodyConfirm
               ("https://res.cloudinary.com/macto/image/upload/v1693393451/Ok-rafiki_fw7rpd.png"
               , $"Hey {subscriber.FirstName} {subscriber.LastName}, thanks for joining us"
               , "your Subscription has been extended successfully");
            _queue.QueueBackgroundWorkItem(async token =>
            {
await _emailSender.SendEmailAsync(subscriber.Email, "Confirm your email", body);
            });
                
          
            
            _context.SaveChanges();
            var viewModel = _mapper.Map<SubscriptionViewModel>(subscription);
            return PartialView("renewalRow", viewModel);
        }
    }

}
