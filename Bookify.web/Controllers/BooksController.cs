using AutoMapper;
using Bookify.web.Core.Const;
using Bookify.web.Core.Models;
using Bookify.web.Core.ViewModel;
using Bookify.web.Data;
using Bookify.web.Filter;
using Bookify.web.settings;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using System.Text.Json.Serialization.Metadata;

namespace Bookify.web.Controllers
{

    [Authorize(Roles = AppRoles.Archive)]
    public class BooksController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly Cloudinary _cloudinarySettings;
        private readonly List<string> _allowedExtentions = new()
        {
            ".jpg",".jpeg",".png"
        };
        private readonly int _maxAllowedSize = 2097152;
        public BooksController(ApplicationDbContext context,
            IMapper mapper,
            IWebHostEnvironment webHostEnvironment,IOptions<CloudinarySettings>  cloudinary)
        {
            Account account = new()
            {
                Cloud=cloudinary.Value.CloudName,
                ApiKey=cloudinary.Value.APIKey,
                ApiSecret=cloudinary.Value.APISecret
            };
            _cloudinarySettings = new Cloudinary(account);
            _mapper = mapper;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult GetBooks()
        {
            int start = int.Parse(Request.Form["start"]!);
            int length = int.Parse(Request.Form["length"]!);
            int sortcolumnindex = int.Parse(Request.Form["order[0][column]"]!);

            string? sortcolumnname = Request.Form[$"columns[{sortcolumnindex}][name]"];
            string? order = Request.Form["order[0][dir]"];
            string? searchvalue = Request.Form["search[value]"];
            IQueryable<Book> books = _context.Books
                .Include(b=>b.Author)
                .Include(b=>b.categories)
                .ThenInclude(b=>b.Category);
            if (!string.IsNullOrEmpty(searchvalue))
            {
                books = books.Where(b => b.Title.Contains(searchvalue) || b.Author.Name.Contains(searchvalue));
            }
            books = books.OrderBy($"{sortcolumnname} {order}");
            var data=books.Skip(start).Take(length).ToList();
            var _mappedData = _mapper.Map<IEnumerable<BookViewModel>>(data);

            int recordsTotal = _context.Books.Count();
            var jsonData = new {recordsFiltered=recordsTotal,recordsTotal,data=_mappedData };
            return Ok(jsonData);
        }
        public IActionResult Details(int id)
        {
            Book? book = _context.Books
                .Include(b=> b.Author)
                .Include(b=>b.copies)
                .Include(b=>b.categories)
                .ThenInclude(b=>b.Category)
                .SingleOrDefault(b =>b.Id == id);
            if (book is null)
            {
                return NotFound();
            }
            BookViewModel viewModel = _mapper.Map<BookViewModel>(book);
           // viewModel.AuthorName = book.Author.Name;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Toggle(int id)
        {
            Book? book = _context.Books.FirstOrDefault(b=>b.Id==id);
            if (book is null) return NotFound();
            book.IsDeleted = !book.IsDeleted;
            book.LastUpdatedOn = DateTime.UtcNow;
            book.LastUpdatedById= User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            book.LastUpdatedById= User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _context.SaveChanges();
            return Ok();
        }
        public IActionResult Create()
        {

            return View("Create", ReturnModels());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookFormViewModel model)
        {

            if (!ModelState.IsValid)
            {

                return View("Create", ReturnModels(model));
            }
            Book book = _mapper.Map<Book>(model);
            foreach (var category in model.SelectedCategories)
            {
                book.categories.Add(new BookCategory
                {
                    CategoryId = category
                });
                Debug.WriteLine(category);
            }


            if (model.Image is not null)
            {
                string extesion = model.Image.FileName;
                if (!_allowedExtentions.Contains(Path.GetExtension(extesion)))
                {
                    ModelState.AddModelError(nameof(model.Image), Core.Const.Error.extensition);
                    return View("Create", ReturnModels(model));
                }
                if (model.Image.Length > _maxAllowedSize)
                {
                    ModelState.AddModelError(nameof(model.Image), Core.Const.Error.maxsize);
                    return View("Create", ReturnModels(model));
                }
                var ImageName = $"{Guid.NewGuid()}{extesion}";
                //var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/Images/Books", ImageName);
                //using var stream = System.IO.File.Create(path);
                //await model.Image.CopyToAsync(stream);
                using var stream = model.Image.OpenReadStream();
                var imageparams = new ImageUploadParams
                {
                    File = new FileDescription(ImageName, stream),
                    UseFilename = true
                };
                var result = await _cloudinarySettings.UploadAsync(imageparams);

                book.ImageUrl = result.SecureUrl.ToString() ;
                book.ImageThumbnailUrl = GetThumbnailurl(book.ImageUrl);
                book.ImagePublicId = result.PublicId;
            }
            book.CreatedById= User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            _context.Books.Add(book);

            _context.SaveChanges();
            //int id = _context.Books.FirstOrDefault(d => d.AuthorId == model.AuthorId && d.Title == model.Title).Id;
            //foreach (var category in model.SelectedCategories)
            //{
            //    _context.BooksCategories.Add(new BookCategory { BookId = id, CategoryId = category });
            //    _context.SaveChanges();
            //}
            // return RedirectToAction("Index");
            return RedirectToAction(nameof(Details), new { id = book.Id});

        }
        public IActionResult Edit(int id)
        {
            
            Book? book = _context.Books.Include(t => t.categories).FirstOrDefault(p => p.Id == id);
            if (book is null) return NotFound();
            var model = _mapper.Map<BookFormViewModel>(book);
            var ViewModel = ReturnModels(model);
            ViewModel.SelectedCategories = book.categories.Select(p => p.CategoryId).ToList();

            return View("Create", ViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>  Edit(BookFormViewModel model)
        {
            Book? book = _context.Books
                .Include(t => t.categories)
                .Include(t=>t.copies)
                .FirstOrDefault(p => p.Id == model.Id);
            if (book is null) return NotFound();

            if (!ModelState.IsValid)
            {

                return View("Create", ReturnModels(model));
            }
            string MyImagePublicId = null;
            if (model.Image is not null)
            {
                if (!string.IsNullOrEmpty(book.ImageUrl))
                {
                    //var OldImagePath = Path.Combine($"{_webHostEnvironment.WebRootPath}/Images/Books", book.ImageUrl);
                    //if(System.IO.File.Exists(OldImagePath)) {
                    //    System.IO.File.Delete(OldImagePath);
                    //}
                    await _cloudinarySettings.DeleteResourcesAsync(book.ImagePublicId);
                }
                string extesion = model.Image.FileName;
                if (!_allowedExtentions.Contains(Path.GetExtension(extesion)))
                {
                    ModelState.AddModelError(nameof(model.Image), Core.Const.Error.extensition);
                    return View("Create", ReturnModels(model));
                }
                if (model.Image.Length > _maxAllowedSize)
                {
                    ModelState.AddModelError(nameof(model.Image), Core.Const.Error.maxsize);
                    return View("Create", ReturnModels(model));
                }
                var ImageName = $"{Guid.NewGuid()}{extesion}";
                //var path = Path.Combine($"{_webHostEnvironment.WebRootPath}/Images/Books", ImageName);
                //using var stream = System.IO.File.Create(path);
                //await model.Image.CopyToAsync(stream);
                using var stream = model.Image.OpenReadStream();
                var imageparams = new ImageUploadParams
                {
                    File = new FileDescription(ImageName, stream),
                    UseFilename = true
                };
                var result = await _cloudinarySettings.UploadAsync(imageparams);

                model.ImageUrl = result.SecureUrl.ToString();

                MyImagePublicId=result.PublicId;
            }
            else if(model.Image is null && !string.IsNullOrEmpty(book.ImageUrl))
            {

                model.ImageUrl = book.ImageUrl; 
                model.ImageThumbnailUrl = book.ImageThumbnailUrl;
            }

            book = _mapper.Map(model,book);
            book.ImageThumbnailUrl = GetThumbnailurl(book.ImageUrl!);
            book.ImagePublicId = MyImagePublicId;
            

            foreach (var category in model.SelectedCategories)
            {
                book.categories.Add(new BookCategory
                {
                    CategoryId = category
                });
                Debug.WriteLine(category);
            }
            book.LastUpdatedOn=DateTime.Now;
            book.LastUpdatedById= User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            if (!model.IsAvailableForRental)
            {
                foreach (var copy in book.copies)
                    copy.IsAvailableForRental = false;
            }
            _context.SaveChanges();
            //int id = _context.Books.FirstOrDefault(d => d.AuthorId == model.AuthorId && d.Title == model.Title).Id;
            //foreach (var category in model.SelectedCategories)
            //{
            //    _context.BooksCategories.Add(new BookCategory { BookId = id, CategoryId = category });
            //    _context.SaveChanges();
            //}
            //return RedirectToAction("Index");
            return RedirectToAction(nameof(Details), new { id = book.Id });

        }
        public IActionResult UniqeItem(BookFormViewModel model) {
            Book? book = _context.Books.FirstOrDefault(b=>b.Title == model.Title && b.AuthorId== model.AuthorId);
            bool exits = (book is null || book.Id.Equals(model.Id)) ;
            return Json(exits);
        }
        public BookFormViewModel ReturnModels(BookFormViewModel? model = null)
        {
            BookFormViewModel ViewModel = (model is null) ? new BookFormViewModel() : model;
            var authors = _context.Authors.Where(b => !b.IsDeleted).OrderBy(b => b.Name).ToList();
            var Categories = _context.Categories.Where(b => !b.IsDeleted).OrderBy(b => b.Name).ToList();

            ViewModel.Authors = _mapper.Map<List<SelectListItem>>(authors);
            ViewModel.Categories = _mapper.Map<List<SelectListItem>>(Categories);
            return ViewModel;

        }
        private string GetThumbnailurl (string url)
        {
            

            string saperator = "/image/upload/";
            var spliturl= url.Split(saperator);
            string result = $"{spliturl[0]}{saperator}c_thumb,w_200,g_face/{spliturl[1]}";
            return result;
        }
    }
}
