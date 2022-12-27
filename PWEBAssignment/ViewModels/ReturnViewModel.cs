using System.ComponentModel.DataAnnotations;
using PWEBAssignment.Models;

namespace PWEBAssignment.ViewModels
{
	public class ReturnViewModel
	{
		public IFormFile PhotoEvidenceFile { get; set; }

		[Display(Name = "Number of KM", Description = "Number of KM that the car had on return", Prompt = "Insert the number of KM that the car had on return")]
		public string NumberOfKm { get; set; }

		[Display(Name = "Vehicle Damage", Description = "The vehicle has been returned with damage (Y/N)", Prompt = "Specify if the vehicle has any damage")]
		public bool VehicleDamage { get; set; }

		[Display(Name = "Observation", Description = "Employe observations", Prompt = "Insert your observations")]
		public string Observations { get; set; }

		public int? EmployeUserId { get; set; }

		public int ReservationId { get; set; }
	}
}