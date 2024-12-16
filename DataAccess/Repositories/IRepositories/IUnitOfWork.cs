using System;
using BusinessLogic.Models;

namespace DataAccess.Repositories.IRepositories;

public interface IUnitOfWork : IDisposable
{
    public IBaseRepository<User> Users { get; }
    public IBaseRepository<Flight> Flights { get; }
    public IBaseRepository<Ticket> Tickets { get; }
    public IBaseRepository<Airplane> Airplanes { get; }
    public IBaseRepository<Offer> Offers { get; }
    public Task<int> CompleteAsync();
}
