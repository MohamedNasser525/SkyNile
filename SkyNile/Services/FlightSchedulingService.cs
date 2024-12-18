using System;
using BusinessLogic.Models;
using DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace SkyNile.Services;

public class FlightSchedulingService : IFlightSchedulingService
{
    private readonly ApplicationDbContext _context;
    public FlightSchedulingService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<DateTime>> GetAvailableFlightTimeScheduleAsync(string airport, DateTime targetDate)
    {
        TimeSpan oneDay = new(24, 0, 0);
        TimeSpan allowedGapBetweenFlights = new(0, 5, 0);
        List<DateTime> bestDateScheduling = new();

        bool isTargetOccupied = await _context.Flights.AnyAsync(f => f.DepartureAirport == airport && f.DepartureTime >= targetDate.Subtract(allowedGapBetweenFlights) && f.DepartureTime <= targetDate.Add(allowedGapBetweenFlights));
        if (!isTargetOccupied)
        {
            bestDateScheduling.Add(targetDate);
            return bestDateScheduling;
        }

        var occupiedFlightsDates = await _context.Flights.Where(f => f.DepartureTime >= targetDate.Subtract(allowedGapBetweenFlights) && f.DepartureTime <= targetDate.Add(oneDay)).
        OrderBy(f => f.DepartureTime).ToListAsync();

        Flight? previousHead = null;
        bool onlyOneLoopIteration = true;
        foreach (var flight in occupiedFlightsDates)
        {
            if (previousHead != null && flight.DepartureTime - previousHead.DepartureTime.
            Add(allowedGapBetweenFlights) >= allowedGapBetweenFlights)
            {
                onlyOneLoopIteration = false;
                bestDateScheduling.Add(previousHead.DepartureTime.Add(allowedGapBetweenFlights));
            }
            previousHead = flight;
        }
        bestDateScheduling.Add(previousHead!.DepartureTime.Add(allowedGapBetweenFlights));
        return bestDateScheduling;
    }
    public async Task DeleteFlightTimeScheduleAsync(Flight f)
    {
        _context.Flights.Remove(f);
        await _context.SaveChangesAsync();
    }
}

