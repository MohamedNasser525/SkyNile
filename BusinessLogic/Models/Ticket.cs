namespace BusinessLogic.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public int TicketCount { get; set; }
        public Guid UserId { get; set; }
        public Guid FlightId { get; set; }
        public double TotalPrice { get; set; }

        public User User { get; set; }
        public Flight Flight { get; set; }
    }

}