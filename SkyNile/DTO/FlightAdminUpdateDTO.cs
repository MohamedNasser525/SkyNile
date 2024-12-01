using System.ComponentModel.DataAnnotations;
using BusinessLogic.CustomAttributes;
namespace SkyNile.DTO;

public class FlightAdminUpdateDTO
{
    public Guid Id { get; set; }
    
    [Display(Name = "Departure Time"), Required(ErrorMessage = "Departure Time is required."), DateRange(90), DataType(DataType.DateTime)]
    public DateTime DepartureTime { get; set; }

    [Display(Name = "Arrival Time"), Required(ErrorMessage = "Estimated Arrival Time is required.")]
    public DateTime ArrivalTime { get; set; }

    [Display(Name = "Base flight price"), Required(ErrorMessage = "Base flight must be specified")]
    public double Price { get; set; }

    [Display(Name = "Flight Status"), Required(ErrorMessage = "Flight status must be specified")]

    public FlightStatus FlightStatus { get; set; }
}