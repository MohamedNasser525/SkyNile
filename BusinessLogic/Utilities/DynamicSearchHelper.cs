using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BusinessLogic.Utilities;
public static class DynamicSearchHelper
{
    public static Expression<Func<T, bool>> BuildSearchExpression<T>(object dto)
    {
        ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
        Expression? combinedExpression = null;

        foreach (PropertyInfo property in dto.GetType().GetProperties())
        {
            // Get value of the property in the DTO
            var value = property.GetValue(dto);
            if (value == null || IsDefaultValue(value)) continue;

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

