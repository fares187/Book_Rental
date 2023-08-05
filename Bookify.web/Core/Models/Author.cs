using System.ComponentModel.DataAnnotations;

namespace Bookify.web.Core.Models
{
	public class Author
	{
		public int Id { get; set; }
		[MaxLength(100)]
		public string Name { get; set; } = null!;
		public bool IsDeleted { get; set; }
		public DateTime JoinedOn { get; set; } = DateTime.UtcNow;
		public DateTime? LastUpdatedOn { get; set; }
	}
}
