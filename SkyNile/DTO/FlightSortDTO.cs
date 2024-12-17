using System;
using System.ComponentModel.DataAnnotations;

namespace SkyNile.DTO;

public class FlightSortDTO
{
    
    public Guid Id { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    
    [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only letters, numbers, and spaces are allowed.")]
    [MaxLength(30, ErrorMessage = "DepartureCountry must be no more 30 characters long")]
    public string DepartureLocation { get; set; }
    
    [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only letters, numbers, and spaces are allowed.")]
    [MaxLength(30, ErrorMessage = "ArrivalLocation must be no more 30 characters long")]
    public string ArrivalLocation { get; set; }
    public Double Price { get; set; }
    public Double Score { get; set; }
}
