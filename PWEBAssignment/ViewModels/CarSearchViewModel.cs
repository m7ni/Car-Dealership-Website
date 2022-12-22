using PWEBAssignment.Models;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;


namespace PWEBAssignment.ViewModels
{
    public class CarSearchViewModel
    {
        public List<Car> ListOfCars { get; set; }
        public int NumResults { get; set; }
        public string textToSearch { get; set; }
    }
}
