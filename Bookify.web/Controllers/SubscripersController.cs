using AutoMapper;
using Bookify.web.Core.Const;
using Bookify.web.Core.Models;
using Bookify.web.Core.ViewModel;
using Bookify.web.Data;

using Bookify.web.Filter;
using Bookify.web.Migrations;
using Bookify.web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Bookify.web.Controllers
{
    [Authorize(Roles = AppRoles.Reception)]
    public class SubscripersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly List<string> _allowedExtentions = new()
        {
            ".jpg",".jpeg",".png"
        };
        private readonly int _maxAllowedSize = 2097152;
        public SubscripersController(ApplicationDbContext context, IMapper mapper, IImageService imageService)
        {
            _context = context;
            _mapper = mapper;
            _imageService = imageService;
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
            _context.Add(sub);
            _context.SaveChanges();
            return RedirectToAction("Index");

        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _context.Subscripers.FirstOrDefault(b => b.Id == id);
            if (user is null)
                return NotFound();
            var ViewModel = _mapper.Map<SubscriperFormViewModel>(user);

            return View("Form", ReturnViewModels(ViewModel));

        }
        [HttpPost]
        public async Task<IActionResult> Edit(SubscriperFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            Subscriper? subscriper = await _context.Subscripers.FirstOrDefaultAsync(b => b.Id == model.Id);
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

            return RedirectToAction("index");
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
        public IActionResult Details(int id)
        {
            var sub = _context.Subscripers.Include(b=>b.Governorate)
                .Include(b=>b.Area)
                .SingleOrDefault(s=>s.Id== id);
            if (sub is null)
                return NotFound();
            var viewModel = _mapper.Map<SubscriperDetailsViewModel>(sub);
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
            var viewModel = _mapper.Map<SubscriperFormViewModel>(subscriper);
            return PartialView("_Result",subscriper);
        }
        public IActionResult UniqeNationalId(SubscriperFormViewModel model)
        {
            if (model.Id > 0)
            {
                bool iswith = _context.Subscripers.Where(c => c.Id != model.Id).Any(c => c.NationalId == model.NationalId);
                return Json(!iswith);
            }
            bool IsAcategory = _context.Subscripers.Any(c => c.NationalId == model.NationalId);
            return Json(!IsAcategory);
        }
        public IActionResult UniqeEmail(SubscriperFormViewModel model)
        {
            if (model.Id > 0)
            {
                bool iswith = _context.Subscripers.Where(c => c.Id != model.Id).Any(c => c.Email == model.Email);
                return Json(!iswith);
            }
            bool IsAcategory = _context.Subscripers.Any(c => c.Email == model.Email);
            return Json(!IsAcategory);
        }
        public IActionResult UniqeMobileNumber(SubscriperFormViewModel model)
        {
            if (model.Id > 0)
            {
                bool iswith = _context.Subscripers.Where(c => c.Id != model.Id).Any(c => c.MobileNumber == model.MobileNumber);
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
    }

}
