using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace PWEBAssignment.Models
{
    public class Deliveries
    {
        [ForeignKey("Reservation")]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Number of KM", Description = "Number of KM that the car had on delivery", Prompt = "Insert the number of KM that the car has on delivery")]
        public string NumberOfKm { get; set; }
        [Required]
        [Display(Name = "Vehicle Damage", Description = "The vehicle is going to be delivered with damage (Y/N)", Prompt = "Specify if the vehicle has any damage")]
        public bool VehicleDamage { get; set; }

        [DataType(DataType.Text)]
		[Display(Name = "Observation", Description = "Employee observations", Prompt = "Insert your observations")]
        public string Observations { get; set; }


        public int? EmployeUserId { get; set; }
        public ApplicationUser EmployeUser { get; set; }
        public int ReservationId { get; set; }
        public Reservations Reservation { get; set; }
    }
}
