using System;

namespace SkyNile.DTO;

public class FlightCrewResponseDTO
{
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public string DepartureLocation { get; set; }
    public string ArrivalLocation { get; set; }

}
