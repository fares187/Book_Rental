using Bookify.web.Core.Models;
using System.ComponentModel.DataAnnotations;

namespace Bookify.web.Core.ViewModel
{
    public class SubscriperDetailsViewModel
    {
        public int Id { get; set; }

        
        public string fullname { get; set; } 
       

        public DateTime dateOfBirth { get; set; }

        public string NationalId { get; set; } = null!;
       
        public string MobileNumber { get; set; } = null!;

        public bool HasWhatApp { get; set; }

        public string Email { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;

        public string ThumbnailUrl { get; set; } = null!;

        public string? Area { get; set; }
    

        public string? Governorate { get; set; }
 

        public string Address { get; set; } = null!;

        public bool IsBlackListed { get; set; }

        public bool IsDeleted { get; set; }

       

        public DateTime CreatedOn { get; set; }



    }
}
