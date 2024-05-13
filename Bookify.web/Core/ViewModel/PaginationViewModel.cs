using Bookify.web.Core.Enums;

namespace Bookify.web.Core.ViewModel
{
    public class PaginationViewModel
    {
        public bool HasPreviousPage { get; set; }
        public int Start {
            get
            {
                var start = 1;
                if (TotalPages > (int)ReportConfigration.maximumPagination)
                {
                   
                        start = (PageNumber - ((int)ReportConfigration.maximumPagination - 1)) < 1 ? 1   : PageNumber - ((int)ReportConfigration.maximumPagination -1)  ;
                  
                }    
                return start;
            }
        }
        public int End {
            get
            {
                var end = TotalPages;
                if (TotalPages > (int)ReportConfigration.maximumPagination)
                    end = (Start+ (int)ReportConfigration.maximumPagination) >=TotalPages ? TotalPages: (Start+(int)ReportConfigration.maximumPagination) ;
                return end;
            }
        }
        public int PageNumber { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage{get;set;}
    }
}
