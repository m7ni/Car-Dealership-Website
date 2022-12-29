using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PWEBAssignment.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Display(Name = "Name", Description = "Company's name", Prompt = "Insert the name of the Company")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
		public string Name { get; set; }

        [Display(Name = "Address", Description = "Company's address", Prompt = "Insert the address of the Company")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
		public string Address { get; set; }

        [Display(Name = "Rating", Description = "Company's Rating", Prompt = "Insert the rating of the Company")]
        public int Rating { get; set; }

        [Display(Name = "Available", Description = "Company's Availability")]
		public bool Available { get; set; }
        public ICollection<ApplicationUser> Workers { get; set; } //can be both Managers or Employee

        public ICollection<Car> Cars { get; set; } //all of the cars that the company runs
   
    }
}
