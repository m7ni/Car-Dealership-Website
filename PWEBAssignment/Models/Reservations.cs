using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace PWEBAssignment.Models
{
    public class Reservations
    {
        public int Id { get; set; }
        public int ClientUserId { get; set; }

        [Display(Name = "Client")]
        public ApplicationUser ClientUser { get; set; }

        [DataType(DataType.Time)]
        [Display(Name = "Reservation Date", Description = "Date of the reservation", Prompt = "Insert the Date of the reservation")]
        public DateTime ReservationDate { get; set; }

		[DataType(DataType.Time)]
		[Display(Name = "Delivery Date", Description = "Delivery Date of the reservation", Prompt = "Insert the Delivery Date of the reservation")]
        public DateTime DeliveryDate { get; set; }

		[DataType(DataType.Time)]
		[Display(Name = "Return Date", Description = "Return Date of the reservation", Prompt = "Insert the Return Date of the reservation")]
        public DateTime ReturnDate { get; set; }

        [Display(Name = "Rejected", Description = "Indicates if the reservation was rejected by the company")]
		public bool Rejected { get; set; }
		[Display(Name = "Return", Description = "Indicates if the user wants to return the car")]
		public bool ConfirmReturn { get; set; }

		[Display(Name = "Price", Description = "Total Price of the reservation")]
		public Double Price { get; set; }
		public int? ReturnId { get; set; }
		public Returns Return { get; set; }
		public int? DeliveryId { get; set; }
		public Deliveries Delivery { get; set; }
		public int? CarId { get; set; }
		public Car Car { get; set; }
		public int? CompanyId { get; set; }
		public Company Company { get; set; }


	}
}
