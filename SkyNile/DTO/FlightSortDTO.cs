using System;
using System.ComponentModel.DataAnnotations;

namespace SkyNile.DTO;

public class FlightSortDTO
{
    
    public Guid Id { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    
    [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only letters, numbers, and spaces are allowed.")]
    [MaxLength(30, ErrorMessage = "Departure Country must be no more 30 characters long")]
    public string DepartureCountry { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only letters, numbers, and spaces are allowed.")]
    [MaxLength(30, ErrorMessage = "Departure Airport must be no more 30 characters long")]
    public string DepartureAirport{ get; set; }


    [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only letters, numbers, and spaces are allowed.")]
    [MaxLength(30, ErrorMessage = "Arrival Country must be no more 30 characters long")]
    public string ArrivalCountry { get; set; }


    [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only letters, numbers, and spaces are allowed.")]
    [MaxLength(30, ErrorMessage = "Arrival Airport must be no more 30 characters long")]
    public string ArrivalAirport { get; set; }

    
    public Double Price { get; set; }
    public Double Score { get; set; }
}
