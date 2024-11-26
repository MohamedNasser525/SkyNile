using System;

namespace SkyNile.Services;

public interface IFlightSchedulingService
{
    
    public Task<IEnumerable<DateTime>> GetAvailableFlightTimeSchedule(DateTime targetDate);

}
