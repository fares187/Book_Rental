using Bookify.web.Core.Models;
using Bookify.web.Core.Utilites;

namespace Bookify.web.Core.ViewModel
{
    public class RentalsReportViewModel
    {
        public DateTime? StartDate { get; set; } 
        public DateTime? EndDate { get; set; }
        public int PageNumber { get; set; }
        public PaginatedList<RentalViewModel> Data { get; set; }
    }
}
