using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Bookify.web.Core.Models
{
	[Index(nameof(NationalId),IsUnique =true)]
	[Index(nameof(Email), IsUnique = true)]
	[Index(nameof(MobileNumber), IsUnique = true)]
	public class Subscriper
	{

		public int Id { get; set; }

		[MaxLength(100)]
		public string FirstName { get; set; } = null!;

		[MaxLength(100)]
		public string LastName { get; set; }=null!;

		public DateTime dateOfBirth { get; set; }
		[MaxLength(20)]
		public string NationalId { get; set; } = null!;
		[MaxLength(15)]
		public string MobileNumber { get; set; } = null!;

		public bool HasWhatApp { get; set; }	
		[MaxLength(150)]
		public string Email { get; set; } = null!;
		[MaxLength(500)]
		public string ImageUrl { get; set; } = null!;
		[MaxLength(500)]
		public string ThumbnailUrl { get; set; } = null!;	

		public int AreaId { get; set; }
		public Area? Area { get; set; }

		public int GovernorateId { get; set; }
		public Governorate? Governorate { get; set; }

		[MaxLength(200)]
		public string Address { get; set; } = null!;

		public bool IsBlackListed { get; set; }

		public bool IsDeleted { get; set; }

		public string? CreatedById { get; set; }

		public ApplicationUser? CreatedBy { get; set; }

		public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

		public string? LastUpdatedById { get; set; }

		public ApplicationUser? LastUpdatedBy { get; set; }

		public DateTime? LastUpdatedOn { get; set; }
	}
}
