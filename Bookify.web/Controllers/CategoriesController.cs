using Bookify.web.Core.Models;
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
    }
}
