using AutoMapper;
using Bookify.web.Core.Const;
using Bookify.web.Core.Models;
using Bookify.web.Core.ViewModel;
using Bookify.web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bookify.web.Controllers
{
    [Authorize(Roles =AppRoles.Archive)]
    public class AuthorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public AuthorsController(ApplicationDbContext context,IMapper mapper)
        {
            _mapper = mapper;   
          _context = context; 
        }
        public IActionResult Index()
        {
            // TODO add viewmodels
            IEnumerable<Author> Authors = _context.Authors.ToList(); 

            return View(nameof(Index),Authors);
        }
        [HttpGet]
        public IActionResult Create()
        { 
            return PartialView("_Form");  
        }
        public IActionResult Create(AuthorForm model)
        {
            Author author = _mapper.Map<Author>(model);
            author.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.Add(author);
            _context.SaveChanges();
           
            return PartialView("_AuthorRow",author);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Author? author = _context.Authors.FirstOrDefault(d => d.Id == id);
            if (author is null)
            {
                return NotFound();
            }
            AuthorForm model =_mapper.Map<AuthorForm>(author);


            return PartialView("_Form",model);
        }
        [HttpPost]
        public IActionResult Edit(AuthorForm model)
        {
            if (!ModelState.IsValid) return BadRequest();
            Author? author = _context.Authors.FirstOrDefault(d => d.Id == model.Id);
            if (author is null)
            {
                return NotFound();
            }
            // TODO unique name maybe
            author.Name = model.Name;
            author.LastUpdatedOn = DateTime.UtcNow;
            author.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.Update(author);
            _context.SaveChanges();
            return PartialView("_AuthorRow",author);
        }
        [HttpPost]
        public IActionResult ToggleStatus(int id)
        {
            Author? author = _context.Authors.Find(id);
            if (author is null)
            {
                return NotFound();
            }
            author.IsDeleted=!(author.IsDeleted);
            author.LastUpdatedOn = DateTime.UtcNow;
            author.LastUpdatedById= User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.Update(author);
            _context.SaveChanges();
            return Ok(author.LastUpdatedOn.ToString());

        }
        public IActionResult UniqeItem(AuthorForm model)
        {
            if (model.Id > 0)
            {
                bool iswith = _context.Authors.Where(c => c.Id != model.Id).Any(c => c.Name == model.Name);
                return Json(!iswith);
            }
            bool IsAcategory = _context.Authors.Any(c => c.Name == model.Name);
            return Json(!IsAcategory);
        }
    }
}
