using System;
using System.ComponentModel.DataAnnotations;

public class ArrivalCheckerAttribute : ValidationAttribute
{
    private readonly string _dependentProperty;

    public ArrivalCheckerAttribute(string dependentProperty)
    {
        _dependentProperty = dependentProperty;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // Get the property we're dependent on (ArrivalLocation in this case)
        var dependentProperty = validationContext.ObjectType.GetProperty(_dependentProperty);

        if (dependentProperty == null)
            throw new ArgumentException("Property with this name not found.");

        var dependentValue = dependentProperty.GetValue(validationContext.ObjectInstance);

        // Check if ArrivalTime (value) is set but ArrivalLocation (dependentValue) is not
        if (value != null && dependentValue == null)
        {
            return new ValidationResult("Location must be assigned with arrival time.");
        }

        return ValidationResult.Success;
    }
}
