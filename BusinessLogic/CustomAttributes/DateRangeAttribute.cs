using System;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogic.CustomAttributes;

public class DateRangeAttribute : ValidationAttribute
{
    private readonly int _daysAhead;

    public DateRangeAttribute(int daysAhead = 60)
    {
        _daysAhead = daysAhead;
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is DateTime date)
        {
            var now = DateTime.Now;
            var maxDate = now.AddDays(_daysAhead);

            // Normalize times to start of day for fair comparison
            date = date.Date;
            now = now.Date;
            maxDate = maxDate.Date;

            if (date < now)
            {
                return new ValidationResult($"Date must not be in the past.");
            }

            if (date > maxDate)
            {
                return new ValidationResult($"Date must not be more than {_daysAhead} days in the future.");
            }

            return ValidationResult.Success;
        }
        return new ValidationResult("Invalid date format.");
    }
}

