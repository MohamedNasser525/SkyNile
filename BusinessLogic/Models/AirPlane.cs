namespace BusinessLogic.Models
{

    public class Airplane
    {
        public Guid Id { get; set; }
        public string Model { get; set; }
        public int Capacity { get; set; }
        public string Airline { get; set; }
        public virtual List<Flight> Flights { get; set; }
    }


}
