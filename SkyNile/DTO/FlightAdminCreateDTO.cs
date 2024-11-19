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

    [Display(Name = "Arrival Location")]
    public string ArrivalLocation { get; set; }
}