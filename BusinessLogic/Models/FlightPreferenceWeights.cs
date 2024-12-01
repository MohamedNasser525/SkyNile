namespace BusinessLogic.Models
{
    public static class FlightPreferenceWeights
    {
        public static readonly Dictionary<FlightPreference, (double PriceWeight, double SpeedWeight)> Weights
            = new Dictionary<FlightPreference, (double, double)>
        {
            { FlightPreference.CheapestFlight, (1.0, 0.0) },
            { FlightPreference.MostlyCheap, (0.7, 0.3) },
            { FlightPreference.Balanced, (0.5, 0.5) },
            { FlightPreference.MostlyFast, (0.3, 0.7) },
            { FlightPreference.FastestFlight, (0.0, 1.0) }
        };

        public static (double PriceWeight, double SpeedWeight) GetWeights(FlightPreference preference)
        {
            return Weights[preference];
        }
    }


}
