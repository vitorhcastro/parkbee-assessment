using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Application.Common.Interfaces;

public interface IParkingDbContext
{
    public DbSet<Door> Doors { get; set; }

    public DbSet<Garage> Garages { get; set; }

    public DbSet<User> Users { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new());

    EntityEntry Entry(object entity);
}
