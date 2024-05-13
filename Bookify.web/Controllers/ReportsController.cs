using AutoMapper;
using Bookify.web.Core.ViewModel;
using Bookify.web.Core.Utilites;
using Bookify.web.Core.Models;
using Bookify.web.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Bookify.web.Core.Enums;
using ClosedXML.Excel;
using OpenHtmlToPdf;
using ViewToHTML.Services;
using DocumentFormat.OpenXml.Spreadsheet;
using Bookify.web.Extentions;
using Microsoft.VisualBasic;
using Bookify.web.Core.Const;
using Humanizer;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Bookify.web.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IMapper _mapper;

        private readonly IViewRendererService _viewRendererService;

        public ReportsController(ApplicationDbContext context, IMapper mapper, IWebHostEnvironment webHostEnvironment, IViewRendererService viewRendererService = null)
        {
            _context = context;
            _mapper = mapper;
            this.webHostEnvironment = webHostEnvironment;
            _viewRendererService = viewRendererService;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Books(List<int>? SelectedAuthors,
           List<int>? SelectedCategories,
           int? pageNumber)
        {
            var authors = _context.Authors
                .OrderBy(a => a.Name)
                .ToList();
            var categories = _context.Categories
               .OrderBy(c => c.Name)
               .ToList();

            IQueryable<Book> books = _context.Books
                .Include(b => b.Author)
                .Include(b => b.categories)
                .ThenInclude(c => c.Category)
                .Where(c => (!SelectedAuthors.Any() || SelectedAuthors.Contains(c.AuthorId)) &&
                (!SelectedCategories.Any() || c.categories.Select(c => c.CategoryId)
                .Any(c => SelectedCategories.Contains(c))));
            //if (SelectedAuthor.Any()){
            //    books = books.Where(c=>SelectedAuthor.Contains(c.AuthorId));
            //}
            var viewModel = new BooksReportViewModel()
            {
                Categories = _mapper.Map<IEnumerable<SelectListItem>>(categories),
                Author = _mapper.Map<IEnumerable<SelectListItem>>(authors)
            };
            if (pageNumber is not null)
                viewModel.Data = PaginatedList<Book>.Create(books, pageNumber ?? 0, (int)ReportConfigration.pagesize);
            return View(viewModel);
        }
        //public IActionResult Rentals(DateTime? startDate, DateTime? EndDate, int? PageNumber)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        //     if(EndDate.)
        //        ModelState.AddModelError("Date", "invalid input");
        //    }

        //    var Rentals = _context.Rentals
        //        .Include(r => r.RentalCopies)
        //        .ThenInclude(c => c.CopyBook)
        //        .ThenInclude(c => c.Book)
        //        .Include(r => r.subscriber)
        //        .Where(b => b.StartDate <= EndDate && b.StartDate >= startDate)
        //        .Select(c => new RentalViewModel()
        //        {
        //            StartDate = c.StartDate,
        //            subscriber = _mapper.Map<SubscriperViewModel>(c.subscriber),
        //            PenaltyPaid = c.PenaltyPaid,
        //            RentalCopies = _mapper.Map<IEnumerable<RentalCopyViewModel>>(c.RentalCopies)

        //        });
        //    var ViewModel = new RentalsReportViewModel();

        //    if (PageNumber is not null)
        //    {
        //        ViewModel.Data = PaginatedList<RentalViewModel>.Create(Rentals, PageNumber ?? 0, (int)ReportConfigration.pagesize);
        //    }
        //    return View(ViewModel);
        //}


        public IActionResult Rentals(string? duration, int? PageNumber)
        {



            var ViewModel = new RentalReportViewModel
            {
                Duration = duration

            };

            if (!string.IsNullOrEmpty(duration))
            {

                string[] dates = duration.Split('-');
                if (!DateTime.TryParse(dates[0], out DateTime from))
                {
                    ModelState.AddModelError("Duration", Error.ReportStartDate);
                    return View(ViewModel);
                }
                if (!DateTime.TryParse(dates[1], out DateTime to))
                {
                    ModelState.AddModelError("Duration", Error.ReportEndDate);
                    return View(ViewModel);
                }
                var Rentals = _context.RentalCopies
               .Include(r => r.Rental)
               .ThenInclude(r => r.subscriber)
               .Include(c => c.CopyBook)
               .ThenInclude(c => c.Book)

               .Where(b => b.RentalDate >= from && b.EndDate <= to)
               .Select(c => new ReportRentalsBooksViewModel()
               {
                   bookName = c.CopyBook.Book.Title,
                   rentalDate = c.RentalDate,
                   endDate = c.EndDate,
                   IsDeleted = c.CopyBook.IsDeleted,
                   subscriber = c.Rental.subscriber.FirstName + " " + c.Rental.subscriber.LastName

               });

                if (PageNumber is not null)

                    ViewModel.Rentals = PaginatedList<ReportRentalsBooksViewModel>.Create(Rentals, PageNumber ?? 0, (int)ReportConfigration.pagesize);




            }

            ModelState.Clear();

            return View("Rentals", ViewModel);
        }
       
        public IActionResult DelayedRentals()
        {

            var DelayedRentals = _context.RentalCopies
                .Include(r => r.Rental)
                .ThenInclude(r => r.subscriber)
                .Include(r => r.CopyBook)
                .ThenInclude(r => r.Book)
                .Where(r=>!r.ReturnDate.HasValue && r.EndDate < DateTime.UtcNow)
                .Select(c => new DelayedRentalsViewModel()
                {
                    BookSerial = c.CopyBook.SerialNumber.ToString(),
                    BookTitle = c.CopyBook.Book.Title,
                    RentalDate = c.RentalDate,
                    EndDate = c.EndDate,
                    SubscriberId = c.Rental.subscriber.Id,
                    SubscriberName = c.Rental.subscriber.FirstName + " " + c.Rental.subscriber.LastName,
                    ExtendedOn = c.ExtendedOn,
                    SubscriberPhone = c.Rental.subscriber.MobileNumber,
                    DelayInDays = (!c.ExtendedOn.HasValue) ? (DateTime.UtcNow - c.EndDate).Days : (DateTime.UtcNow - c.ExtendedOn.Value.AddDays((int) RentalConfigurations.RentalDuration)).Days
                }).ToList() ;
            return  View("DelayedRentalsView", DelayedRentals);
        }
        public async Task<ActionResult> ExportDelayedRentalsToExcel()
        {
            var DelayedRentals = _context.RentalCopies
               .Include(r => r.Rental)
               .ThenInclude(r => r.subscriber)
               .Include(r => r.CopyBook)
               .ThenInclude(r => r.Book)
               .Where(r => !r.ReturnDate.HasValue && r.EndDate < DateTime.UtcNow)
               .Select(c => new DelayedRentalsViewModel()
               {
                   BookSerial = c.CopyBook.SerialNumber.ToString(),
                   BookTitle = c.CopyBook.Book.Title,
                   RentalDate = c.RentalDate,
                   EndDate = c.EndDate,
                   SubscriberId = c.Rental.subscriber.Id,
                   SubscriberName = c.Rental.subscriber.FirstName + " " + c.Rental.subscriber.LastName,
                   ExtendedOn = c.ExtendedOn,
                   SubscriberPhone = c.Rental.subscriber.MobileNumber,
                   DelayInDays = (!c.ExtendedOn.HasValue) ? (DateTime.UtcNow - c.EndDate).Days : (DateTime.UtcNow - c.ExtendedOn.Value.AddDays((int)RentalConfigurations.RentalDuration)).Days
               }).ToList();

            var heads = new string[] { "Subscriber ID","Subscriber Name","Subscriber phone","Book Title",
                "Book Serial","Rental Date","End Date","Extended On","Delay in Dayes"};
            var workBook = new XLWorkbook();
            var sheet = workBook.AddWorksheet("Rental");
            sheet.AddHeaders(heads);

            for (int i = 0; i < DelayedRentals.Count(); i++)
            {
                sheet.Cell(i + 2, 1).SetValue(DelayedRentals[i].SubscriberId);
                sheet.Cell(i + 2, 2).SetValue(DelayedRentals[i].SubscriberName);
                sheet.Cell(i + 2, 3).SetValue(DelayedRentals[i].SubscriberPhone);
                sheet.Cell(i + 2, 4).SetValue(DelayedRentals[i].BookTitle);
                sheet.Cell(i + 2, 5).SetValue(DelayedRentals[i].BookSerial);
                sheet.Cell(i + 2, 6).SetValue(DelayedRentals[i].RentalDate);
                sheet.Cell(i + 2, 7).SetValue(DelayedRentals[i].EndDate);
                sheet.Cell(i + 2, 8).SetValue(DelayedRentals[i].ExtendedOn);
                sheet.Cell(i + 2, 9).SetValue(DelayedRentals[i].DelayInDays);
               
            }
            sheet.formatsheet();

            var stream = new MemoryStream();
            workBook.SaveAs(stream);
            return File(stream.ToArray(), "application/octet-stream", $"DelayedRentals_{DateTime.Now}.xlsx");

        }

        public async Task<IActionResult> ExportDelayedRentalsToPDF()
        {
            var DelayedRentals = _context.RentalCopies
              .Include(r => r.Rental)
              .ThenInclude(r => r.subscriber)
              .Include(r => r.CopyBook)
              .ThenInclude(r => r.Book)
              .Where(r => !r.ReturnDate.HasValue && r.EndDate < DateTime.UtcNow)
              .Select(c => new DelayedRentalsViewModel()
              {
                  BookSerial = c.CopyBook.SerialNumber.ToString(),
                  BookTitle = c.CopyBook.Book.Title,
                  RentalDate = c.RentalDate,
                  EndDate = c.EndDate,
                  SubscriberId = c.Rental.subscriber.Id,
                  SubscriberName = c.Rental.subscriber.FirstName + " " + c.Rental.subscriber.LastName,
                  ExtendedOn = c.ExtendedOn,
                  SubscriberPhone = c.Rental.subscriber.MobileNumber,
                  DelayInDays = (!c.ExtendedOn.HasValue) ? (DateTime.UtcNow - c.EndDate).Days : (DateTime.UtcNow - c.ExtendedOn.Value.AddDays((int)RentalConfigurations.RentalDuration)).Days
              }).ToList();



            var html = await _viewRendererService.RenderViewToStringAsync(ControllerContext,
               "~/Views/Reports/DelayedReportspd.cshtml",
                DelayedRentals);

            var pdf = Pdf.From(html).Content();

            return File(pdf.ToArray(), "application/octet-stream", $"DelayedRentals_{DateTime.Now}.pdf");
        }
        // ToDo: export Rentals to excel and pdf
        public async Task<IActionResult> ExportRentalsToExcel(string duration)
        {
            var ViewModel = new RentalReportViewModel
            {
                Duration = duration

            };
            if (string.IsNullOrEmpty(duration))
            {
                ModelState.AddModelError("Duration", Error.ReportStartDate);
                return View(ViewModel);
            }
            if (!DateTime.TryParse(duration.Split("-")[0], out DateTime startDate))
            {
                ModelState.AddModelError("Duration", Error.ReportStartDate);
                return View(ViewModel);
            }
            if (!DateTime.TryParse(duration.Split("-")[1], out DateTime EndDate))
            {
                ModelState.AddModelError("Duration", Error.ReportEndDate);
                return View(ViewModel);
            }

            var Rentals = _context.RentalCopies
              .Include(r => r.Rental)
              .ThenInclude(r => r.subscriber)
              .Include(c => c.CopyBook)
              .ThenInclude(c => c.Book)

              .Where(b => b.RentalDate >= startDate && b.EndDate <= EndDate)
              .Select(c => new ReportRentalsBooksViewModel()
              {
                  bookName = c.CopyBook.Book.Title,
                  rentalDate = c.RentalDate,
                  endDate = c.EndDate,
                  IsDeleted = c.CopyBook.IsDeleted,
                  subscriber = c.Rental.subscriber.FirstName + " " + c.Rental.subscriber.LastName

              }).ToList();
            var heads = new string[] { "Book Title","Rental Date","End Date","Is Deleted?",
                "Subscriber full name"};
            var workBook = new XLWorkbook();
            var sheet = workBook.AddWorksheet("Rental");
            sheet.AddHeaders(heads);

            for (int i = 0; i < Rentals.Count(); i++)
            {
                sheet.Cell(i + 2, 1).SetValue(Rentals[i].bookName);
                sheet.Cell(i + 2, 2).SetValue(Rentals[i].rentalDate.ToString("dd MMM, yyyy"));
                sheet.Cell(i + 2, 3).SetValue(Rentals[i].endDate.ToString("dd MMM, yyyy"));
                sheet.Cell(i + 2, 4).SetValue(Rentals[i].IsDeleted ? "Yes" : "No");
                sheet.Cell(i + 2, 5).SetValue(Rentals[i].subscriber);
            }
            sheet.formatsheet();

            var stream = new MemoryStream();
            workBook.SaveAs(stream);
            return File(stream.ToArray(), "application/octet-stream", $"Rental_{DateTime.Now}.xlsx");


        }








        public async Task<IActionResult> ExportBooksToExcel(List<int>? SelectedAuthors,
          List<int>? SelectedCategories)
        {

            var books = _context.Books
                .Include(b => b.Author)
                .Include(b => b.categories)
                .ThenInclude(c => c.Category)
                .Where(c => (!SelectedAuthors.Any() || SelectedAuthors.Contains(c.AuthorId))
                && (!SelectedCategories.Any() || c.categories.Select(c => c.CategoryId).Any(c => SelectedCategories.Contains(c))))
                .ToList();
            var heads = new string[] { "Title","Author","Categories","Publisher",
                "Publishing Date","Hall","Rental status","Status"};
            var workBook = new XLWorkbook();
            var sheet = workBook.AddWorksheet("Books");
            sheet.AddHeaders(heads);

            for (int i = 0; i < books.Count(); i++)
            {
                sheet.Cell(i + 2, 1).SetValue(books[i].Title);
                sheet.Cell(i + 2, 2).SetValue(books[i].Author.Name);
                sheet.Cell(i + 2, 3).SetValue(string.Join(" , ", books[i].categories.Select(c => c.Category.Name)));
                sheet.Cell(i + 2, 4).SetValue(books[i].Publisher);
                sheet.Cell(i + 2, 5).SetValue(books[i].PublishingDaTe.ToString("dd MMM,yyyy"));
                sheet.Cell(i + 2, 6).SetValue(books[i].Hall);
                sheet.Cell(i + 2, 7).SetValue(books[i].IsAvailableForRental ? "yes" : "no");
                sheet.Cell(i + 2, 8).SetValue(books[i].IsDeleted ? "deleted" : "avaliable");
            }
            sheet.formatsheet();

            var stream = new MemoryStream();
            workBook.SaveAs(stream);
            return File(stream.ToArray(), "application/octet-stream", $"Book_{DateTime.Now}.xlsx");
        }
        public async Task<IActionResult> ExportBooksToPDF(List<int>? SelectedAuthors,
        List<int>? SelectedCategories)
        {

            var books = _context.Books
                .Include(b => b.Author)
                .Include(b => b.categories)
                .ThenInclude(c => c.Category)
                .Where(c => (!SelectedAuthors.Any() || SelectedAuthors.Contains(c.AuthorId))
                && (!SelectedCategories.Any() || c.categories.Select(c => c.CategoryId).Any(c => SelectedCategories.Contains(c))))
                .ToList();
            #region --OldPDFCode
            //var html = await System.IO.File.ReadAllTextAsync(webHostEnvironment.WebRootPath.ToString() + "/assats/templates/Reports.html");
            //html = html
            //    .Replace("[Title]", "Books");
            //var body = "<table>" +
            //    "<thead>" +
            //    "<tr>" +
            //    "<th>Title</th>" +
            //    "<th>Author</th>" +
            //    "<th>Categories</th>" +
            //    "<th>Publisher</th>" +
            //    "" +
            //    "" +
            //    "</tr>" +
            //    "</thead>" +
            //    "<tbody>"

            //    ;
            //foreach (var book in books)
            //{
            //    body += "<tr>" +
            //        "<td>" +
            //        book.Title +
            //        "</td>" +
            //         "<td>" +
            //        book.Author.Name +
            //        "</td>" +
            //         "<td>" +
            //        string.Join(" , ", book.categories.Select(c => c.Category.Name)) +
            //        "</td>" +
            //         "<td>" +
            //        book.Publisher +
            //        "</td>" +
            //        "</tr>";
            //}
            //body += "</tbody></table>";

            //html = html.Replace("[Body]",body);


            #endregion


            var html = await _viewRendererService.RenderViewToStringAsync(ControllerContext,
               "~/Views/Reports/Report.cshtml",
                _mapper.Map<IEnumerable<BookViewModel>>(books));

            var pdf = Pdf.From(html).Content();

            return File(pdf.ToArray(), "application/octet-stream", $"Book_{DateTime.Now}.pdf");
        }


        //public async Task<IActionResult> GetHTML()
        //{
        //    var templatePath = "~/assats/templates/Reports.html";
        //    var html = await _viewRendererService.RenderViewToStringAsync(ControllerContext, templatePath, _books);

        //    return Ok(html);

        //}
    }
}
