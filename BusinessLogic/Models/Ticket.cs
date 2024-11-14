namespace BusinessLogic.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public int TicketCount { get; set; }
        public Guid PassengerId { get; set; }
        public Guid FlightId { get; set; }
        public double Price { get; set; }

        public Passenger Passenger { get; set; }
        public Flight Flight { get; set; }
    }

}