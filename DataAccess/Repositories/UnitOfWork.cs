using System;
using BusinessLogic.Models;
using DataAccess.Data;
using DataAccess.Repositories.IRepositories;

namespace DataAccess.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public IBaseRepository<User> Users { get; private set; }
    public IBaseRepository<Flight> Flights { get; private set; }
    public IBaseRepository<Ticket> Tickets { get; private set; }
    public IBaseRepository<Airplane> Airplanes { get; private set; }
    public IBaseRepository<Offer> Offers { get; }
    
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Users = new BaseRepository<User>(_context);Flights = new BaseRepository<Flight>(_context);
        Tickets = new BaseRepository<Ticket>(_context);Airplanes = new BaseRepository<Airplane>(_context);
    }
    public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();
    public void Dispose() => _context.Dispose();
}
