using System;

namespace SkyNile.DTO;

public class FlightSortDTO
{
    
    public Guid Id { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public Double Price { get; set; }
    public Double Score { get; set; }
}
