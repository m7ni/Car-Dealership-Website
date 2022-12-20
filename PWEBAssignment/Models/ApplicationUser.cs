using Microsoft.AspNetCore.Identity;

namespace PWEBAssignment.Models
{
	public class ApplicationUser : IdentityUser
	{
		public String firstName { get; set; }
		public String lastName { get; set; }
	}
}
