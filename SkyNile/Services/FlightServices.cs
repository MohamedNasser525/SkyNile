using System;
using BusinessLogic.Models;
using DataAccess.Repositories.IRepositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SkyNile.Services.Interfaces;

namespace SkyNile.Services;

public class FlightServices : IFlightServices
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMailingServices _mailingServices;
    private readonly UserManager<User> _userManager;
    public FlightServices(IUnitOfWork unitOfWork, IMailingServices mailingServices, UserManager<User> userManager)
    {
        _unitOfWork = unitOfWork;
        _mailingServices = mailingServices;
        _userManager = userManager;
    }

    public async Task NotifyDeletedFlightSubscribersAsync(Flight flightToDelete, IEnumerable<Ticket> ticketsToNotify)
    {
        foreach (var ticket in ticketsToNotify)
        {
            ticket.TicketStatus = TicketStatus.CancelledWithRefund;
            var userIdToNotify = ticket.UserId;
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userIdToNotify.ToString());
            var body = await _mailingServices.PrepareFlightCancellationBodyAsync(user!, flightToDelete, ticket);
            await _mailingServices.SendMailAsync(user!.Email!, "Flight Cancellation Notice", body, null);
        }
        flightToDelete.FlightStatus = FlightStatus.Cancelled;
        await _unitOfWork.CompleteAsync();
    }
}
