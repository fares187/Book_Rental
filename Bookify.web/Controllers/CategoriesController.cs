using Bookify.web.Core.Models;
using Bookify.web.Core.ViewModel;
using Bookify.web.Data;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.web.Controllers
{
    public class CategoriesController : Controller
    {
        public readonly ApplicationDbContext _context;
        public CategoriesController(ApplicationDbContext context)
        {
            this._context = context;
        }
    
        public IActionResult Index()
        {
            //TODO:use ViewModel
            IEnumerable<Category> categories = _context.Categories.ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CreateCategoriesViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            _context.Add(new Category()
            {
                Name = model.Name,
                IsDeleted = false
            });
            _context.SaveChanges();
            TempData["Massage"] = "Saved successfully";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Edit(int id) {
            Category category = _context.Categories.Find(id);
            if(category is null)
            {
                return NotFound();
            }
            CreateCategoriesViewModel create = new CreateCategoriesViewModel()
            {
                id = id,
                Name = category.Name
            };
            return View("Create",create); 
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CreateCategoriesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", model);
            }
            Category category = _context.Categories.Find(model.id);

            if (category is null)
            {
                return NotFound();
            }
            category.Name = model.Name;
            category.LastUpdatedOn = DateTime.UtcNow;
            _context.SaveChanges();
            TempData["Massage"] = "Saved successfully";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Toggle(int id)
        {
            
            Category cat = _context.Categories.Find(id);
            if(cat is null)
            {
                return NotFound();
            }
            cat.IsDeleted = !cat.IsDeleted;
            cat.LastUpdatedOn = DateTime.UtcNow;
            _context.SaveChanges();
            return Ok(cat.LastUpdatedOn.ToString());
        }
    }
}
