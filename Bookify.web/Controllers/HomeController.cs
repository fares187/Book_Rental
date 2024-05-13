using AutoMapper;
using Bookify.web.Core.Const;
using Bookify.web.Core.ViewModel;
using Bookify.web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Diagnostics;
using System.Security.Claims;

namespace Bookify.web.Controllers
{

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
private readonly IDataProtector _dataProtector;
 private readonly ILogger<HomeController> _logger;
        


       

        public HomeController(ILogger<HomeController> logger
            , ApplicationDbContext context, IMapper mapper
            ,IDataProtectionProvider dataProtectionProvider)
        {
            _context = context;
            _mapper = mapper;
        
            _logger = logger;
            _dataProtector = dataProtectionProvider.CreateProtector("MySecureKey");
        }

        public IActionResult Index()
        {
            string? userid = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userid is not null)
                return RedirectToAction("Index",controllerName:"DashBoard");
            var lastAddBooks = _context.Books
               .Include(b => b.Author)
               .OrderByDescending(c => c.JoinedOn)
               .Where(b => !b.IsDeleted)
               .Take(10)
               .ToList();
            var viewModel = _mapper.Map<IEnumerable<BookViewModel>>(lastAddBooks);
            foreach (var book in viewModel)
            {
                book.Key = _dataProtector.Protect(book.Id.ToString());
            }
            return View(viewModel);
        }
       


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}