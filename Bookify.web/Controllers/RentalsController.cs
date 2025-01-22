using AutoMapper;
using Bookify.web.Areas.Identity.Pages;
using Bookify.web.Core.Const;
using Bookify.web.Core.Enums;
using Bookify.web.Core.Models;
using Bookify.web.Core.ViewModel;
using Bookify.web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client; 
using System.Security.Claims;

namespace Bookify.web.Controllers
{
    [Authorize(Roles = AppRoles.Reception)]
    public class RentalsController : Controller
    {
        private readonly ApplicationDbContext _Context;
        private readonly IMapper _mapper;
        private readonly IDataProtector _dataProtector;


        public RentalsController(ApplicationDbContext context, IDataProtectionProvider dataProtectionProvider, IMapper mapper)
        {
            _Context = context;
            _mapper = mapper;
            _dataProtector = dataProtectionProvider.CreateProtector("MySecureKey");
        }

        public IActionResult Create(string Key)
        {
            var subscriberId = int.Parse(_dataProtector.Unprotect(Key));
            var subscriber = _Context.Subscripers
                .Include(x => x.Subscriptions)
                .Include(x => x.Rentals)
                .ThenInclude(r => r.RentalCopies)
                .SingleOrDefault(x => x.Id == subscriberId);

            if (subscriber is null)
                return NotFound();


            var (errorMassage, MaxAllowedCopies) = allowSubscriber(subscriber);
            if (!string.IsNullOrEmpty(errorMassage))
            {
                return View("NotAllowedRental", errorMassage);
            }


            var viewModel = new RentalFormViewModel
            {

                SubscriberKey = Key,
                MaxAllowedCopies = (int)MaxAllowedCopies

            };
            return View("Form", viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RentalFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);
            var subscriberId = int.Parse(_dataProtector.Unprotect(model.SubscriberKey));
            var subscriber = _Context.Subscripers
                .Include(x => x.Subscriptions)
                .Include(x => x.Rentals)
                .ThenInclude(r => r.RentalCopies)
                .SingleOrDefault(x => x.Id == subscriberId);

            if (subscriber is null)
                return NotFound();

            var (errorMassage, MaxAllowedCopies) = allowSubscriber(subscriber);
            if (!string.IsNullOrEmpty(errorMassage))

                return View("NotAllowedRental", errorMassage);
            /*
            var currentSubscriberRentals = _Context.Rentals
                .Include(c => c.RentalCopies)
                .ThenInclude(c => c.CopyBook)
                .Where(c => c.SubscriberId == subscriberId)
                .SelectMany(c => c.RentalCopies)
                .Where(c => !c.ReturnDate.HasValue)
                .Select(c => c.CopyBook.BookId)
                .ToList();

            var selectedCopies = _Context.BookCopies.
                Include(b => b.Book)
                .Include(b => b.Rentals)
                .Where(c => model.SelectedCopies.Contains(c.SerialNumber)).ToList();
            List<RentalCopy> copies = new();
            foreach (var copy in selectedCopies)
            {
                if (!copy.IsAvailableForRental || !copy.Book.IsAvailableForRental)

                    return View("NotAllowedRental", Error.rentalError);
                if (copy.Rentals.Any(c => !c.ReturnDate.HasValue))
                    return View("NotAllowedRental", Error.CopyIsInRental);
                if (currentSubscriberRentals.Any(BookId => BookId == copy.BookId))
                    return View("NotAllowedRental", $"this subscriber already has a copy for '{copy.Book.Title}' Book");
                copies.Add(new RentalCopy() { CopyBookId = copy.Id });
            }*/

            var (errors, copies) = getCopies(subscriberId, model);
            if (!string.IsNullOrEmpty(errors))
                return View("NotAllowedRental", errors);
            Rental rental = new()
            {
                RentalCopies = copies!,
                CreatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            };
            subscriber.Rentals.Add(rental);
            _Context.SaveChanges();
            return RedirectToAction("Details", new { id = rental.Id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetCopyDetails(SearchFormViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var copy = _Context.BookCopies.Include(c => c.Book)
                .SingleOrDefault(c => c.SerialNumber.ToString() == model.Value
                && !c.IsDeleted && !c.Book.IsDeleted);
            if (copy is null)
                return NotFound(Error.serialNumber.ToString());
            if (!copy.IsAvailableForRental || !copy.Book.IsAvailableForRental)
                return BadRequest(Error.rentalError);

            //check if copy in not in rental
            var copyIsInRental = _Context.RentalCopies.Any(c => c.CopyBookId == copy.Id && !c.ReturnDate.HasValue);
            if (copyIsInRental)
            {
                return BadRequest(Error.CopyIsInRental);
            }

            var ViewModel = _mapper.Map<BookCopyViewModel>(copy);
            return PartialView("_copyDetails", ViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarkAsDeleted(int id)
        {
            var rental = _Context.Rentals.Find(id);
            if (rental is null || rental.CreatedOn.Date != DateTime.Today)
                return NotFound();
            rental.IsDeleted = true;
            rental.LastUpdatedOn = DateTime.UtcNow;
            rental.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            _Context.SaveChanges();
            return Ok();
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var rental = _Context.Rentals
                .Include(r => r.RentalCopies)
                .ThenInclude(c => c.CopyBook)
                .ThenInclude(c => c.Book)
                .SingleOrDefault(r => r.Id == id);
            if (rental is null)
                return NotFound();
            var ViewModel = new RentalDetailsViewModel()
            {
                id = rental.Id,

                Copies = rental.RentalCopies.Select(c => new RentalCopyDetailsViewModel()
                {
                    RentalDate = c.RentalDate,
                    EndDate = c.EndDate,
                    ExtendedOn = c.ExtendedOn,

                    ReturnDate = c.ReturnDate,
                    ImageUrl = c.CopyBook.Book.ImageUrl,
                    ImageThumbnailUrl = c.CopyBook.Book.ImageThumbnailUrl,
                    Title = c.CopyBook.Book.Title
                }),
                RentalDate = rental.CreatedOn
            };
            return View("Details", ViewModel);

        }
        [HttpGet]

        public IActionResult Edit(int id)
        {
            var Rental = _Context.Rentals
                .Include(r => r.RentalCopies)
                .ThenInclude(c => c.CopyBook)
                .SingleOrDefault(r => r.Id == id);
            if (Rental is null || Rental.CreatedOn.Date != DateTime.Today)
                return NotFound();

            var subscriber = _Context.Subscripers
                .Include(x => x.Subscriptions)
                .Include(x => x.Rentals)
                .ThenInclude(r => r.RentalCopies)
                .SingleOrDefault(x => x.Id == Rental.SubscriberId);

            if (subscriber is null)
                return NotFound();

            var (errorMassage, MaxAllowedCopies) = allowSubscriber(subscriber, Rental.Id);
            if (!string.IsNullOrEmpty(errorMassage))
                return View("NotAllowedRental", errorMassage);
            var currentCopiesIds = Rental.RentalCopies.Select(c => c.CopyBookId).ToList();
            var currentCopies = _Context.BookCopies
                .Where(c => currentCopiesIds.Contains(c.Id))
                .Include(c => c.Book)
                .ToList();
            RentalFormViewModel ViewModel = new()
            {
                SubscriberKey = _dataProtector.Protect(Rental.SubscriberId.ToString()),
                MaxAllowedCopies = (int)MaxAllowedCopies!,
                CurrentCopies = _mapper.Map<IEnumerable<BookCopyViewModel>>(currentCopies)
            };
            return View("Form", ViewModel);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(RentalFormViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);
            var Rental = _Context.Rentals
                .Include(r => r.RentalCopies)
                .SingleOrDefault(r => r.Id == model.Id);

            if (Rental is null || Rental.CreatedOn.Date != DateTime.Today)
                return NotFound();

            var subscriberId = int.Parse(_dataProtector.Unprotect(model.SubscriberKey));

            var subscriber = _Context.Subscripers
               .Include(x => x.Subscriptions)
               .Include(x => x.Rentals)
               .ThenInclude(r => r.RentalCopies)
               .SingleOrDefault(x => x.Id == subscriberId);

            if (subscriber is null)
                return NotFound();

            var (errorMassage, MaxAllowedCopies) = allowSubscriber(subscriber, model.Id);
            if (!string.IsNullOrEmpty(errorMassage))

                return View("NotAllowedRental", errorMassage);
            /*
            var currentSubscriberRentals = _Context.Rentals
                .Include(c => c.RentalCopies)
                .ThenInclude(c => c.CopyBook)
                .Where(c => c.SubscriberId == subscriberId && c.Id != model.Id)
                .SelectMany(c => c.RentalCopies)
                .Where(c => !c.ReturnDate.HasValue)
                .Select(c => c.CopyBook.BookId)
                .ToList();

            var selectedCopies = _Context.BookCopies.
                Include(b => b.Book)
                .Include(b => b.Rentals)
                .Where(c => model.SelectedCopies.Contains(c.SerialNumber)).ToList();
            List<RentalCopy> copies = new();
            foreach (var copy in selectedCopies)
            {
                if (!copy.IsAvailableForRental || !copy.Book.IsAvailableForRental)

                    return View("NotAllowedRental", Error.rentalError);
                if (copy.Rentals.Any(c => !c.ReturnDate.HasValue && c.RentalId != model.Id))
                    return View("NotAllowedRental", Error.CopyIsInRental);
                if (currentSubscriberRentals.Any(BookId => BookId == copy.BookId))
                    return View("NotAllowedRental", $"this subscriber already has a copy for '{copy.Book.Title}' Book");
                copies.Add(new RentalCopy() { CopyBookId = copy.Id });
            }*/

            var (errors, copies) = getCopies(subscriberId, model);
            if (!string.IsNullOrEmpty(errors))
                return View("NotAllowedRental", errors);
            Rental.RentalCopies = copies!;
            Rental.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            Rental.LastUpdatedOn = DateTime.UtcNow;
            _Context.SaveChanges();
            return RedirectToAction("Details", new { id = Rental.Id });
        }
        public IActionResult Return(int id)
        {
            var Rental = _Context.Rentals
              .Include(r => r.RentalCopies)
              .ThenInclude(c => c.CopyBook)
              .ThenInclude(c => c!.Book)
              .SingleOrDefault(r => r.Id == id);
            if (Rental is null || Rental.CreatedOn.Date == DateTime.Today)
                return NotFound();

            var subscriber = _Context.Subscripers
                .Include(x => x.Subscriptions)
                .SingleOrDefault(x => x.Id == Rental.SubscriberId);
            var ViewModel = new RentalReturnFormViewModel()
            {
                Id = id,
                rentalCopies = _mapper.Map<IList<RentalCopyViewModel>>(Rental.RentalCopies.Where(c=>!c.ReturnDate.HasValue)).ToList(),
                SelectedCopies = Rental.RentalCopies.Where(c => !c.ReturnDate.HasValue).Select(c => new ReturnCopyViewModel()
                {
                    Id = c.CopyBookId,
                    IsReturned = c.ExtendedOn.HasValue ? false : null
                }).ToList(),
                AllowExtend = !subscriber!.IsBlackListed
                && subscriber!.Subscriptions.Last().EndDate.Date >= Rental.StartDate.AddDays(((int)RentalConfigurations.RentalDuration) * 2)
                && Rental.StartDate.AddDays(((int)RentalConfigurations.RentalDuration)) >= DateTime.Today
            };
            return View(ViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Return(RentalReturnFormViewModel model)
        {
            var Rental = _Context.Rentals
             .Include(r => r.RentalCopies)
             .ThenInclude(c => c.CopyBook)
             .ThenInclude(c => c!.Book)
             .SingleOrDefault(r => r.Id == model.Id);
            if (Rental is null || Rental.CreatedOn.Date == DateTime.Today)
                return NotFound();
            if (!ModelState.IsValid)
            {
                model.rentalCopies = _mapper.Map<IList<RentalCopyViewModel>>(Rental.RentalCopies.Where(c => !c.ReturnDate.HasValue)).ToList();

                return View(model);
            }

            var subscriber = _Context.Subscripers
                .Include(x => x.Subscriptions)
                .SingleOrDefault(x => x.Id == Rental.SubscriberId);
            if (model.SelectedCopies.Any(c => c.IsReturned.HasValue && !c.IsReturned.Value))
            {
                string error = string.Empty;
                if (subscriber.IsBlackListed)
                    error = Error.blackListedrental;
                else if (subscriber!.Subscriptions.Last().EndDate.Date < Rental.StartDate.AddDays(((int)RentalConfigurations.RentalDuration) * 2))
                    error = Error.RentalIsNotAllowdForInActive;
                else if (Rental.StartDate.AddDays(((int)RentalConfigurations.RentalDuration)) < DateTime.Today)
                    error = Error.ExtendNotAllowed;

                if (!string.IsNullOrEmpty(error))
                {
                    model.rentalCopies = _mapper.Map<IList<RentalCopyViewModel>>(Rental.RentalCopies.Where(c => !c.ReturnDate.HasValue)).ToList();
                    ModelState.AddModelError("", error);
                    return View(model);
                }
            }
            var isUpdated = false;
            foreach (var copy in model.SelectedCopies)
            {
                if (!copy.IsReturned.HasValue) continue;
                var currentCopy = Rental.RentalCopies.SingleOrDefault(c => c.CopyBookId == copy.Id);
                if (currentCopy is null) continue;
                if (copy.IsReturned.HasValue && copy.IsReturned.Value)
                {
                    if (currentCopy.ReturnDate.HasValue)
                        continue;
                    currentCopy.ReturnDate = DateTime.UtcNow;
                    isUpdated=true;
                }
                if (copy.IsReturned.HasValue && !copy.IsReturned.Value)
                {
                    if (currentCopy.ExtendedOn.HasValue)
                        continue;
                    currentCopy.ExtendedOn = DateTime.UtcNow;
                    currentCopy.EndDate = currentCopy.EndDate.AddDays((int)RentalConfigurations.RentalDuration) ;
                    isUpdated = true;
                }
            }
            if (isUpdated)
            {
                Rental.LastUpdatedById = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                Rental.LastUpdatedOn= DateTime.UtcNow;
                Rental.PenaltyPaid = model.PenaltyPaid;
                _Context.SaveChanges();
            }

            return RedirectToAction("Details", new { id = Rental.Id });
        }
        public (string errorMassage, int? MaxAllowedCopies) allowSubscriber(Subscriper subscriber, int? rentalId = null)
        {
            if (subscriber.IsBlackListed)
                //  return View("NotAllowedRental", Error.BlackListed);
                return (errorMassage: Error.BlackListed, null);
            if (subscriber.Subscriptions.Last().EndDate < DateTime.Today.AddDays((int)RentalConfigurations.RentalDuration))

                return (errorMassage: Error.inactiveSubscriber, null);
            var currentRentals = subscriber.Rentals
                .Where(r => rentalId == null || r.Id != rentalId)
                .SelectMany(r => r.RentalCopies)
                .Count(b => !b.ReturnDate.HasValue);

            var AvaliableCopiesCount = (int)RentalConfigurations.MaxAllowedCopies - currentRentals;

            if (AvaliableCopiesCount.Equals(0))

                return (errorMassage: Error.maxallowedReached, null);
            return (string.Empty, AvaliableCopiesCount);
        }
        private (string errorMassage, List<RentalCopy>? recopies) getCopies
            (int subscriberId, RentalFormViewModel model)
        {
            var currentSubscriberRentals = _Context.Rentals
                .Include(c => c.RentalCopies)
                .ThenInclude(c => c.CopyBook)
                .Where(c => c.SubscriberId == subscriberId && (model.Id == null || c.Id != model.Id))
                .SelectMany(c => c.RentalCopies)
                .Where(c => !c.ReturnDate.HasValue)
                .Select(c => c.CopyBook.BookId)
                .ToList();

            var selectedCopies = _Context.BookCopies.
                Include(b => b.Book)
                .Include(b => b.Rentals)
                .Where(c => model.SelectedCopies.Contains(c.SerialNumber)).ToList();
            List<RentalCopy> copies = new();
            foreach (var copy in selectedCopies)
            {
                if (!copy.IsAvailableForRental || !copy.Book.IsAvailableForRental)

                    // return View("NotAllowedRental", Error.rentalError);
                    return (errorMassage: Error.rentalError, recopies: null);
                if (copy.Rentals.Any(c => !c.ReturnDate.HasValue && (c.RentalId != model.Id || model.Id == null)))
                    //  return View("NotAllowedRental", Error.CopyIsInRental);
                    return (errorMassage: Error.CopyIsInRental, recopies: null);
                if (currentSubscriberRentals.Any(BookId => BookId == copy.BookId))
                    //  return View("NotAllowedRental", $"this subscriber already has a copy for '{copy.Book.Title}' Book");
                    return (errorMassage: $"this subscriber already has a copy for '{copy.Book.Title}' Book"
                        , recopies: null);
                copies.Add(new RentalCopy() { CopyBookId = copy.Id });
            }
            return (errorMassage: string.Empty, recopies: copies);
        }


    }

}
