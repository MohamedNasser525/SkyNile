using System;
using BusinessLogic.Models;

namespace SkyNile.Services.Interfaces;

public interface IFlightServices
{
    public Task NotifyDeletedFlightSubscribersAsync(Flight flightToDelete, IEnumerable<Ticket> ticketsToNotify);
}
