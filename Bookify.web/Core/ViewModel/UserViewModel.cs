namespace Bookify.web.Core.ViewModel
{
    public class UserViewModel
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public bool IsDeleted { get; set; }
    }
}
