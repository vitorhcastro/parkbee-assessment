using System.Reflection;
using Application.Common.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ParkingDbContext : DbContext, IParkingDbContext
{
    private readonly ICurrentUserService currentUserService;

    private readonly IDateTime dateTime;

    public ParkingDbContext(
        DbContextOptions<ParkingDbContext> options,
        ICurrentUserService currentUserService,
        IDateTime dateTime)
        : base(options)
    {
        this.currentUserService = currentUserService;
        this.dateTime = dateTime;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        var entries = this.ChangeTracker.Entries<AuditableEntity>();
        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = this.currentUserService.UserId;
                    entry.Entity.Created = this.dateTime.Now;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = this.currentUserService.UserId;
                    entry.Entity.LastModified = this.dateTime.Now;
                    break;
            }
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        return result;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }

    public DbSet<Door> Doors { get; set; }
    public DbSet<Garage> Garages { get; set; }
    public DbSet<User> Users { get; set; }
}
