using System.ComponentModel.DataAnnotations;
using BusinessLogic.CustomAttributes;
namespace SkyNile.DTO;

public class FlightAdminCreateDTO
{
    [Display(Name = "Departure Time"), Required(ErrorMessage = "Departure Time is required."), DateRange(60), DataType(DataType.DateTime)]
    public DateTime DepartureTime { get; set; }

    [Display(Name = "Departure Location"), Required(ErrorMessage = "Departure Location is required.")]
    public string DepartureLocation { get; set; }

    [Display(Name = "Arrival Time"), Required(ErrorMessage = "Estimated Arrival Time is required.")]
    public DateTime ArrivalTime { get; set; }

    [Display(Name = "Arrival Location"), Required(ErrorMessage = "Arrival Location is required.")]
    public string ArrivalLocation { get; set; }

    [Display(Name = "Available Seats number"), Required(ErrorMessage = "Seats number must be specified")]
    public int Seatsnum { get; set; }

    [Display(Name = "Base flight price"), Required(ErrorMessage = "Base flight must be specified")]
    public double Price { get; set; }

    [Display(Name = "Airplane type Id"), Required(ErrorMessage = "Airplane type must be specified")]
    public Guid AirplaneId { get; set; }
}