using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PWEBAssignment.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Display(Name = "Name", Description = "Company's name", Prompt = "Insert the name of the Company")]
        public string Name { get; set; }

        [Display(Name = "Address", Description = "Company's address", Prompt = "Insert the address of the Company")]
        public string Address { get; set; }

        [Display(Name = "Rating", Description = "Company's Rating", Prompt = "Insert the rating of the Company")]
        public int Rating { get; set; }

        public bool Available { get; set; }
        public ICollection<ApplicationUser> Workers { get; set; } //can be both Managers or Employee

        public ICollection<Car> Cars { get; set; } //all of the cars that the company runs
   
    }
}
