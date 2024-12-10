using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.Models
{

    public class Airplane
    {
        public Guid Id { get; set; }

        [MaxLength(50)]
        public string Model { get; set; }
        public int Capacity { get; set; }

        [MaxLength(50)]
        public string Airline { get; set; }
        public virtual List<Flight> Flights { get; set; }
    }


}
