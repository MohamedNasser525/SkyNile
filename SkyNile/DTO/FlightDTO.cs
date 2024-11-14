using System;
using System.ComponentModel.DataAnnotations;
using BusinessLogic.Models;
namespace SkyNile.DTO;

public class FlightDTO
{
    [Required(ErrorMessage = "Departure Time is required.")]
    public DateTime DepartureTime { get; set; }

    [ArrivalChecker("ArrivalLocation")]
    public DateTime? ArrivalTime { get; set; }

    [Required(ErrorMessage = "Departure Location is required.")]
    public string DepartureLocation { get; set; }
    public string? ArrivalLocation { get; set; }
}
