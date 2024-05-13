using AutoMapper;
using Bookify.web.Core.ViewModel;
using Bookify.web.Data;
using Bookify.web.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.View;

namespace Bookify.web.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        private readonly ApplicationDbContext _context;
		private readonly IDataProtector _dataProtector;
		private readonly IMapper _mapper;

        public DashBoardController(IDataProtectionProvider dataProtectionProvider,ApplicationDbContext context, IMapper mapper = null)
        {
			_dataProtector = dataProtectionProvider.CreateProtector("MySecureKey");
			_context = context;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
          ///  throw new Exception("can not work");
            var lastAddBooks = _context.Books
                .Include(b => b.Author)
                .OrderByDescending(c => c.JoinedOn)
                .Where(b => !b.IsDeleted)
                .Take(8)
                .ToList();


            var TopBooks = _context.RentalCopies
                .Include(r => r.CopyBook)
                .ThenInclude(c => c.Book)
                .ThenInclude(c => c.Author)
                .GroupBy(c => new
                {
                    c.CopyBook.BookId,
                    c.CopyBook.Book.Title,
                    c.CopyBook.Book.ImageThumbnailUrl,
                    AuthorName = c.CopyBook.Book.Author.Name,

                })
                .Select(b => new
                {
                    b.Key.BookId,
                    b.Key.Title,
                    b.Key.ImageThumbnailUrl,
                    b.Key.AuthorName,
                    count = b.Count()
                })
                .OrderByDescending(b => b.count)
                .Take(8)
                .Select(b => new BookViewModel
                {
                    Key = _dataProtector.Protect(b.BookId.ToString()) ,
                    ImageThumbnailUrl = b.ImageThumbnailUrl,
                    Title = b.Title,
                    AuthorName = b.AuthorName,
                })
                .ToList();

            var ViewModel = new DashBoardViewModel()
            {
                BookCopiesCount = justifyInt(_context.BookCopies.Count(c => !c.IsDeleted)),
                SubscriberCount = justifyInt(_context.Subscripers.Count(c => !c.IsDeleted)),
                LastAddedBooks = _mapper.Map<IEnumerable<BookViewModel>>(lastAddBooks),
                TopBooks = TopBooks
            };
            return View(ViewModel);
        }
        [AjaxOnly]
        public IActionResult GetRentalPerDay(DateTime? startDate = null, DateTime? endDate = null)
        {
            
                startDate ??= DateTime.Today.AddDays(-29);
                endDate ??= DateTime.Today;
            

            var data = _context.RentalCopies
                .Where(c => c.RentalDate.Date >= startDate && c.RentalDate.Date <= endDate)
                .GroupBy(c => c.RentalDate)
                .Select(c => new ChartViewModel
                {
                    text = c.Key.ToString("d MMM"),
                    value = c.Count().ToString()
                })
                .ToList();
            List<ChartViewModel> figures = new();
            for (DateTime day = startDate.Value; day <= endDate; day = day.AddDays(1))
            {

                var daydata = data
                    .SingleOrDefault(s => s.text.Equals(day.ToString("d MMM")));

                figures.Add(new()
                {
                    text = day.ToString("d MMM"),
                    value = (daydata is not null) ? daydata.value : "0"
                });


            }
            return Ok(figures);
        }
        public IActionResult getSubscribersPerCity()
        {
            var subscribers = _context.Subscripers
                .Where(c => !c.IsDeleted && !c.IsBlackListed)
                .GroupBy(c => c.Governorate)
                .Select(c => new
                {
                    text= c.Key!.Name,
                    value=c.Count(),

                })
                .ToList();
            return Ok(subscribers);
        }
        private int justifyInt(int num)
        {
            return (num <= 10 ? num : (num / 10) * 10);
        }
    }
}
