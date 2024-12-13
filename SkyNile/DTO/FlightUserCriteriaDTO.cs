using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BusinessLogic.CustomAttributes;
namespace SkyNile.DTO;

public class FlightUserCriteriaDTO
{
    [Display(Name = "Departure Time"), Required(ErrorMessage = "Departure Time is required."), DateRange(60), DataType(DataType.DateTime)]
    public DateTime DepartureTime { get; set; }

    [Display(Name = "Departure Country"), Required(ErrorMessage = "Departure Country is required.")]
    public string DepartureCountry { get; set; } = "";

    [Display(Name = "Departure Airport"), Required(ErrorMessage = "Departure Country is required.")]
    public string DepartureAirport { get; set; } = "";

    [Display(Name = "Arrival Country")]
    public string? ArrivalCountry { get; set; }

    [Display(Name = "Arrival Airport")]
    public string? ArrivalAirport { get; set; } 
}
