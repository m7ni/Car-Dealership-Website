using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PWEBAssignment.Models
{
    public class Car
    {
        public int Id { get; set; }

        [Display(Name = "Model", Description = "Model of the car", Prompt = "Insert the Model of the car")]
        public string Model { get; set; }

        [Display(Name = "License Plate", Description = "License Plate of the car", Prompt = "Insert the License Plate of the car")]
        public string LicencePlate { get; set; }

        [Display(Name = "Damage", Description = "Damage on the car (Y/N)", Prompt = "Specify if the car is damaged")]
        public bool Damage { get; set; }

        [Display(Name = "Available", Description = "Car's Availability (Y/N)", Prompt = "Insert the availability of the car")]
        public bool Available { get; set; }

        [Display(Name = "Company", Description = "Car's Availability (Y/N)", Prompt = "Insert the availability of the car")]
        public int? CompanyID { get; set; }
        public Company Company { get; set; }

        [Display(Name = "Category", Description = "Car's Availability (Y/N)", Prompt = "Insert the availability of the car")]
        public int? CategoryID { get; set; }
        public Category Category { get; set; }
    }
}
