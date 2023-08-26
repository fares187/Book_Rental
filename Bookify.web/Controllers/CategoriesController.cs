using AutoMapper;
using Bookify.web.Core.Const;
using Bookify.web.Core.Models;
using Bookify.web.Core.ViewModel;
using Bookify.web.Data;
using Bookify.web.Filter;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bookify.web.Controllers
{
    [Authorize(Roles = AppRoles.Archive)]
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public CategoriesController(ApplicationDbContext context,IMapper mapper)
        {
            this._context = context;
            this._mapper= mapper; 
        }
    
        public IActionResult Index()
        {
            //TODO:use ViewModel

            //IEnumerable<CategoryViewModel> categoriesViews = _context.Categories.Select(v=>new CategoryViewModel
            //{
            //    Id= v.Id,   
            //    Name = v.Name,
            //    CreatedOn= v.CreatedOn,
            //    IsDeleted= v.IsDeleted, 
            //    LastUpdatedOn= v.LastUpdatedOn

            //}).AsNoTracking().ToList();

            IEnumerable<Category> categoriesViews = _context.Categories.AsNoTracking().ToList();
            IEnumerable <CategoryViewModel> ViewModel= _mapper.Map<IEnumerable<CategoryViewModel>>(categoriesViews);
            return View(ViewModel);
        }
        [AjaxOnly]
        public IActionResult Create()
        {
            return PartialView("_Form");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateCategoriesViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            Category catee = _mapper.Map<Category>(model);
            catee.LastUpdatedById= User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.Add(catee);
            _context.SaveChanges();
            //CategoryViewModel view = new CategoryViewModel()
            //{
            //    Name = catee.Name,
            //    IsDeleted = catee.IsDeleted,
            //    CreatedOn = catee.CreatedOn,
            //    Id = catee.Id,
            //    LastUpdatedOn = catee.LastUpdatedOn
            //};
            CategoryViewModel view = _mapper.Map<CategoryViewModel>(catee);
            TempData["Massage"] = "Saved successfully";
            return PartialView("_CategoryRow",view );
        }
        [AjaxOnly]
        public IActionResult Edit(int id) {
            Category category = _context.Categories.Find(id);
            if(category is null)
            {
                return NotFound();
            }
            CreateCategoriesViewModel create   = _mapper.Map<CreateCategoriesViewModel>(category); 
          //  return View("_Form",create); 
          return PartialView("_Form", create);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CreateCategoriesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            Category category = _context.Categories.Find(model.id);

            if (category is null)
            {
                return NotFound();
            }
            // category.Name = model.Name;
            category = _mapper.Map(model, category);
            category.LastUpdatedOn = DateTime.UtcNow;
            category.LastUpdatedById= User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.SaveChanges();
            //CategoryViewModel view = new CategoryViewModel()
            //{
            //    Name = category.Name,
            //    IsDeleted = category.IsDeleted,
            //    CreatedOn = category.CreatedOn,
            //    Id = category.Id,
            //    LastUpdatedOn = category.LastUpdatedOn
            //};
            CategoryViewModel view= _mapper.Map<CategoryViewModel>(category);
            TempData["Massage"] = "Saved successfully";
            return PartialView("_CategoryRow", view);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Toggle(int id)
        {
            
            Category? cat = _context.Categories.FirstOrDefault(d=> d.Id== id);
            if(cat is null)
            {
                return NotFound();
            }
            cat.IsDeleted = !cat.IsDeleted;
            cat.LastUpdatedOn = DateTime.UtcNow;
            cat.LastUpdatedById= User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.SaveChanges();
            return Ok(cat.LastUpdatedOn.ToString());
        }
        public IActionResult UniqeItem(CreateCategoriesViewModel model) {
            if(model.id>0)
            {
                bool iswith=_context.Categories.Where(c=> c.Id!=model.id).Any(c=>c.Name==model.Name);
                return Json(!iswith);
            }
            bool IsAcategory= _context.Categories.Any(c=> c.Name==model.Name);
            return Json(!IsAcategory);
        }
    }
}
