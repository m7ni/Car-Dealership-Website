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

        public int? CompanyId { get; set; }
        public Company Company { get; set; }

        public int? CarId { get; set; }
        public Car Car { get; set; }

        [Display(Name = "Delivery Date", Description = "Delivery Date of the reservation", Prompt = "Insert the Delivery Date of the reservation")]
        public DateTime DeliveryDate { get; set; }

        [Display(Name = "Return Date", Description = "Return Date of the reservation", Prompt = "Insert the Return Date of the reservation")]
        public DateTime ReturnDate { get; set; }
        public int? ReturnId { get; set; }
        public Returns Return { get; set; }
        public int? DeliveryId { get; set; }
        public Deliveries Delivery { get; set; }
        public bool Rejected { get; set; }
        public bool ConfirmReturn { get; set; }
        public Double Price { get; set; }
	}
}
