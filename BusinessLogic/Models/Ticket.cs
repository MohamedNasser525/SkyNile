namespace BusinessLogic.Models
{

    public enum TicketStatus
    {
        Pending,
        Used,
        Expired,
        CancelledWithRefund,
        Cancelled
    }
    public class Ticket
    {
        public Guid Id { get; set; }
        public int TicketCount { get; set; }
        public Guid UserId { get; set; }
        public Guid FlightId { get; set; }
        public double TotalPrice { get; set; }
        public TicketStatus TicketStatus { get; set; }
        public virtual User User { get; set; }
        public virtual Flight Flight { get; set; }
    }

}