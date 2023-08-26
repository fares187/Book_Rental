using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Bookify.web.Core.Models
{
	[Index(nameof(Name),IsUnique =true)]
	public class Governorate
	{
		public int Id { get; set; }
		[MaxLength(100)]
		public string Name { get; set; } = null!;

		public ICollection<Area> Areas { get; set; }=new List<Area>();	
		public bool IsDeleted { get; set; }

		public string? CreatedById { get; set; }

		public ApplicationUser? CreatedBy { get; set; }

		public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

		public string? LastUpdatedById { get; set; }

		public ApplicationUser? LastUpdatedBy { get; set; }

		public DateTime? LastUpdatedOn { get; set; }
	}
}
