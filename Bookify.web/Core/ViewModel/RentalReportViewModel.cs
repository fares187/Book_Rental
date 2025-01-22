using Bookify.web.Core.Models;
using Bookify.web.Core.Utilites;

namespace Bookify.web.Core.ViewModel
{
    public class RentalReportViewModel
    {
        public string Duration { get; set; } = null;
        public PaginatedList<ReportRentalsBooksViewModel> Rentals { get; set; }
    }
}
