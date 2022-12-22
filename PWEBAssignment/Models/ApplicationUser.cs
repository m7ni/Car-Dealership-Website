using Microsoft.AspNetCore.Identity;

namespace PWEBAssignment.Models
{
	public class ApplicationUser : IdentityUser
	{
		public String firstName { get; set; }
		public String lastName { get; set; }
        public bool available { get; set; }

        public int? CompanyID { get; set; }
        public Company Company { get; set; }
    }
}
