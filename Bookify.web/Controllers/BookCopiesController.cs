using AutoMapper;
using Bookify.web.Core.Const;
using Bookify.web.Core.Models;
using Bookify.web.Core.ViewModel;
using Bookify.web.Data;
using Bookify.web.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Bookify.web.Controllers
{
    [Authorize(Roles = AppRoles.Archive)]
    public class BookCopiesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        public BookCopiesController( ApplicationDbContext context,IMapper mapper)
        {
            _mapper = mapper;
            _context = context;       
        }
        public IActionResult Index()
        {
            return View();
        }
        [AjaxOnly]
        [HttpGet]
        public IActionResult Create(int BookId)
        {
            var book = _context.Books.FirstOrDefault(b=>b.Id==BookId);
            if(book is null)
            {
                return NotFound();  
            }
            BookCopyFormViewModel form = new BookCopyFormViewModel() { 
                BookId = BookId,
                showRentalInput = book.IsAvailableForRental
            };
          
            
            return PartialView("Form",form);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BookCopyFormViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest();

            var book = _context.Books.FirstOrDefault(b => b.Id == model.BookId);
            if (book is null) return NotFound();

            var copy = new BookCopy()
            {

                EditionNumber = model.EditionNumber,
                IsAvailableForRental = book.IsAvailableForRental ? model.IsAvailableForRental : false
            };
            copy.CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            book.copies.Add(copy);
            _context.SaveChanges() ;

            BookCopyViewModel view = _mapper.Map<BookCopyViewModel>(copy);
            return PartialView("_BookCopyRow", view);

        }
        public IActionResult Edit(int id)
        {
            BookCopy? copy = _context.BookCopies.Include(b=>b.Book).FirstOrDefault(b=>b.Id== id);
            if (copy is null)
            {
                return NotFound();
            }
            BookCopyFormViewModel form = new BookCopyFormViewModel()
            {
                Id=copy.Id,
                showRentalInput = copy.Book.IsAvailableForRental,
                EditionNumber= copy.EditionNumber,
                IsAvailableForRental= copy.IsAvailableForRental 
            };
            //  return View("_Form",create); 
            return PartialView("Form", form);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BookCopyFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            BookCopy? copy = _context.BookCopies.Find(model.Id);

            if (copy is null)
            {
                return NotFound();
            }
            
            copy = _mapper.Map(model, copy);
            copy.LastUpdatedOn = DateTime.UtcNow;
            copy.LastUpdatedById= User.FindFirst(ClaimTypes.NameIdentifier)!.Value; 
            _context.SaveChanges();
       
            BookCopyViewModel view = _mapper.Map<BookCopyViewModel>(copy);

            return PartialView("_BookCopyRow", view);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleStatus(int id)
        {
            BookCopy? copy = _context.BookCopies.Find(id);
            if (copy is null)
            {
                return NotFound();
            }
            copy.IsDeleted = !(copy.IsDeleted);
            copy.LastUpdatedOn = DateTime.UtcNow;
            copy.LastUpdatedById= User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.SaveChanges();
            return Ok(copy.LastUpdatedOn.ToString());
        }

    }
}
