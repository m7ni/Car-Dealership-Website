using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace PWEBAssignment.Models
{
    public class Category
    {
        public int Id { get; set; }


        [Display(Name = "Name", Description = "Car's Category", Prompt = "Insert the category of the car")]
        public string Name { get; set; }
        [Display(Name = "Price", Description = "Car's Reservation Price per day (Y/N)", Prompt = "Insert the price of the car")]
        public decimal PriceHour { get; set; }

        public ICollection<Car> Car { get; set; }
    }
}
