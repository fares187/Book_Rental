using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Bookify.web.Core.Models
{
    [Index(nameof(Email),IsUnique=true)]
    [Index(nameof(UserName), IsUnique = true)]
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100)]
        public string FullName { get; set; } = null!;

        public bool IsDeleted { get; set; }

        public string? CreatedById { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public string? LastUpdatedById { get; set; }

        public DateTime? LastUpdatedOn { get; set; }



    }
}
