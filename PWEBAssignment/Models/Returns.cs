using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PWEBAssignment.Models
{
    public class Returns
    {
        [ForeignKey("Reservation")]
        public int Id { get; set; }

        [Display(Name = "Number of KM", Description = "Number of KM that the car had on return", Prompt = "Insert the number of KM that the car had on return")]
        public string NumberOfKm { get; set; }

        [Display(Name = "Vehicle Damage", Description = "The vehicle has been returned with damage (Y/N)", Prompt = "Specify if the vehicle has any damage")]
        public bool VehicleDamage { get; set; }

        public byte[]? PhotoEvidence { get; set; }

        [Display(Name = "Observation", Description = "Employe observations", Prompt = "Insert your observations")]

        public string Observations { get; set; }

        public int? EmployeUserId { get; set; }
        public ApplicationUser EmployeUser { get; set; }

        public int ReservationId { get; set; }
        public Reservations Reservation { get; set; }
    }
}
