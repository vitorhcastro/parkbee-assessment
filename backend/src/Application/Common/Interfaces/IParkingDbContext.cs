using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IParkingDbContext
{
    public DbSet<Door> Doors { get; set; }

    public DbSet<Garage> Garages { get; set; }

    public DbSet<User> Users { get; set; }
    public DbSet<ParkingSession> ParkingSessions { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new());

    void SetModified(object entity);
}
