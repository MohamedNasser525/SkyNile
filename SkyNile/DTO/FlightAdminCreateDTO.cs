using System.ComponentModel.DataAnnotations;
using BusinessLogic.CustomAttributes;
namespace SkyNile.DTO;

public class FlightAdminCreateDTO
{
    [Display(Name = "Departure Time"), Required(ErrorMessage = "Departure Time is required."), DateRange(90)]
    public DateTime DepartureTime { get; set; }

    [Display(Name = "Arrival Time"), Required(ErrorMessage = "Estimated Arrival Time is required.")]
    public DateTime ArrivalTime { get; set; }

    [Display(Name = "Departure Country"), Required(ErrorMessage = "Departure Country is required.")]
    [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only letters, numbers, and spaces are allowed.")]
    [MaxLength(50, ErrorMessage = "DepartureCountry must be no more 50 characters long")]
    public string DepartureCountry { get; set; } = "";


    [Display(Name = "Departure Airport"), Required(ErrorMessage = "Departure Country is required.")]
    [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only letters, numbers, and spaces are allowed.")]
    [MaxLength(50, ErrorMessage = "DepartureCountry must be no more 50 characters long")]
    public string DepartureAirport { get; set; } = "";

    [Display(Name = "Arrival Country")]
    [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only letters, numbers, and spaces are allowed.")]
    [MaxLength(50, ErrorMessage = "ArrivalCountry must be no more 50 characters long")]
    public string? ArrivalCountry { get; set; }

    [Display(Name = "Arrival Airport")]
    [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only letters, numbers, and spaces are allowed.")]
    [MaxLength(50, ErrorMessage = "ArrivalAirport must be no more 50 characters long")]
    public string? ArrivalAirport { get; set; }


    [Display(Name = "Available Seats number"), Required(ErrorMessage = "Seats number must be specified")]
    [RegularExpression(@"^[0-9]*$", ErrorMessage = "Only numbers allowed.")]
    public int Seatsnum { get; set; }

    [Display(Name = "Base flight price"), Required(ErrorMessage = "Base flight must be specified")]
    public double Price { get; set; }

    [Display(Name = "Airplane type Id"), Required(ErrorMessage = "Airplane type must be specified")]
    public Guid AirplaneId { get; set; }
}