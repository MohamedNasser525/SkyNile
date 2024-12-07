using System;
using System.Linq.Expressions;
using System.Reflection;
using BusinessLogic.Models;
using SkyNile.DTO;
using SkyNile.Services.Interfaces;

namespace SkyNile.Services;

public class FlightSearchService : ISearchService
{
    // UserPreferences, Flights

    private double NormalizePrice(double value, double min, double max) => min == max? 1 : (value - min) / (max - min);
    private double NormalizeDuration(TimeSpan value, TimeSpan min, TimeSpan max) {
        double valueTicks = value.Ticks, minTicks = min.Ticks, maxTicks = max.Ticks; 
        return min == max? 1 : (valueTicks - minTicks) / (maxTicks - minTicks);
    }

    /// <summary>
    /// *This method calculate an equation score and then sort the flights that will be shown to the customer based on equation score using 
    /// *Customer price and duration weights preferences
    /// *The equation = priceWeight * normalizedPrice + durationWeight * normalizedDuration
    /// *The flight that has the minimum score value from the equation is the Best
    /// *the flight that has the maximum score value from the equation is the Worst
    /// * Finally return the sorted list of flights based on score property.
    /// </summary>
    /// <param name="flights"></param>
    /// <param name="preference"></param>
    /// *<returns> List of Sorted Flights based on the score calculated in this method</returns>
    public IEnumerable<FlightSortDTO> SortFlightsByUserPreference(IEnumerable<FlightSortDTO> flights, FlightPreference preference)
    {
        (double priceWeight, double durationWeight) = FlightPreferenceWeights.GetWeights(preference);
        double minPrice = flights.Min(f => f.Price), maxPrice = flights.Max(f => f.Price);
        TimeSpan minDuration = flights.Min(f => f.ArrivalTime - f.DepartureTime), maxDuration = 
        flights.Max(f => f.ArrivalTime - f.DepartureTime);

        foreach (var flight in flights){
            var normalizedPrice = NormalizePrice(flight.Price, minPrice, maxPrice);
            var normalizedDuration = NormalizeDuration(flight.ArrivalTime - flight.DepartureTime, minDuration, maxDuration);
            flight.Score = priceWeight * normalizedPrice + durationWeight * normalizedDuration;
        }
        return flights.OrderBy(f => f.Score);

    }

    public Expression<Func<T, bool>> BuildSearchExpression<T>(object dto)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        Expression? combinedExpression = null;

        foreach (PropertyInfo property in dto.GetType().GetProperties())
        {
            // Get value of the property in the DTO
            var value = property.GetValue(dto);
            if (value == null || IsDefaultValue(value) || (value is string strValue && string.IsNullOrWhiteSpace(strValue))) continue;

            // Create a member expression for the target entity property
            var entityProperty = typeof(T).GetProperty(property.Name);
            if (entityProperty == null) continue; // Skip if the entity does not have this property

            var member = Expression.Property(parameter, entityProperty);

            // Convert the value to the correct type and create a constant expression
            var constant = Expression.Constant(value, entityProperty.PropertyType);

            // Create an equality expression (e.g., x.Property == value)
            var equality = Expression.Equal(member, constant);

            // Combine with previous expressions using Expression.AndAlso
            combinedExpression = combinedExpression == null
                ? equality
                : Expression.AndAlso(combinedExpression, equality);
        }

        // If no properties were filled, return a default true expression
        return combinedExpression == null
            ? x => true
            : Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
    }

    // Helper method to check if a value is the default for its type
    private static bool IsDefaultValue(object value)
    {
        var type = value.GetType();
        return value.Equals(type.IsValueType ? Activator.CreateInstance(type) : null);
    }

}
