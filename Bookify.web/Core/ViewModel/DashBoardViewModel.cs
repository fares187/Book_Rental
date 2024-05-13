namespace Bookify.web.Core.ViewModel
{
    public class DashBoardViewModel
    {
        public int SubscriberCount { get; set; }
        public int BookCopiesCount { get; set; }
        public IEnumerable<BookViewModel> LastAddedBooks { get; set; }=new List<BookViewModel>();
        public IEnumerable<BookViewModel> TopBooks { get; set; }=new List<BookViewModel>();


    }
}
