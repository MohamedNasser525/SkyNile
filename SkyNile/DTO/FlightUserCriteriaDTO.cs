using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BusinessLogic.CustomAttributes;
using Microsoft.AspNetCore.Mvc.ModelBinding;
namespace SkyNile.DTO;

public class FlightUserCriteriaDTO
{
    [Display(Name = "Departure Time"), Required(ErrorMessage = "Departure Time is required."), DateRange(60), DataType(DataType.DateTime)]
    public DateTime DepartureTime { get; set; }

    [Display(Name = "Departure Country"), Required(ErrorMessage = "Departure Country is required.")]
    [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only letters, numbers, and spaces are allowed.")]
    [MaxLength(30, ErrorMessage = "DepartureCountry must be no more 30 characters long")]
    public string DepartureCountry { get; set; } = "";

    [Display(Name = "Departure Airport"), Required(ErrorMessage = "Departure Country is required.")]
    [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only letters, numbers, and spaces are allowed.")]
    [MaxLength(30, ErrorMessage = "DepartureAirport must be no more 30 characters long")]
    public string DepartureAirport { get; set; } = "";

    [Display(Name = "Arrival Country")]
    [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only letters, numbers, and spaces are allowed.")]
    [MaxLength(30, ErrorMessage = "ArrivalCountry must be no more 30 characters long")]
    public string? ArrivalCountry { get; set; }

    [Display(Name = "Arrival Airport")]
    [RegularExpression(@"^[a-zA-Z0-9\s]*$", ErrorMessage = "Only letters, numbers, and spaces are allowed.")]
    [MaxLength(30, ErrorMessage = "ArrivalAirport must be no more 30 characters long")]
    public string? ArrivalAirport { get; set; }
    [BindNever]
    public FlightStatus FlightStatus { get; } =  FlightStatus.Scheduled;
}
