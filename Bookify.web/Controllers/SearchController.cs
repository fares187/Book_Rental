using AutoMapper;
using Bookify.web.Core.Models;
using Bookify.web.Core.ViewModel;
using Bookify.web.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookify.web.Controllers
{
	public class SearchController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IDataProtector _dataProtector;
		private readonly IMapper _mapper;
		public SearchController(IMapper mapper
			,ApplicationDbContext context, IDataProtectionProvider dataProtectionProvider)
		{
			_mapper = mapper;
			_context = context;
			_dataProtector = dataProtectionProvider.CreateProtector("MySecureKey");
		}
		public IActionResult Index() {

			return View();
		}
		public IActionResult Find(string Typedm) {
			var book = _context.Books
				.Include(b=>b.Author)
				.Where(b=>!b.IsDeleted && (b.Title.Contains(Typedm)|| b.Author!.Name.Contains(Typedm)))
				.Select(b => new
                {
					title=b.Title,
                    Author=b.Author.Name,
					key=_dataProtector.Protect(b.Id.ToString())
				}).ToList();
			return Ok(book);
		}
		public IActionResult Details(string Key)
		{
			var bookId =int.Parse(_dataProtector.Unprotect(Key));
			
			var Book = _context.Books
				.Include(b=>b.copies)
				.Include(b=>b.Author)
				.Include(b=>b.categories)
				.ThenInclude(b=>b.Category)
				.SingleOrDefault(b=>b.Id==bookId && !b.IsDeleted);
			if(Book is null)
				return NotFound();
			var ViewModol = _mapper.Map<BookViewModel>(Book);
			ViewModol.Key = Key;
			return View("Details",ViewModol);
			
		}
	}
}
