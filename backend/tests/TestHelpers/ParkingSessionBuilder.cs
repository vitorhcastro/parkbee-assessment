using Domain.Common;
using Domain.Entities;
using static TestHelpers.UserBuilder;

namespace TestHelpers;

public class ParkingSessionBuilder
{
    private Guid id = Guid.NewGuid();
    private User user = AUser().Build();
    private DateTime startDate = DateTime.UtcNow;
    private DateTime? endDate;
    private ParkingSessionStatus status = ParkingSessionStatus.Running;

    public static ParkingSessionBuilder AParkingSession()
    {
        return new ParkingSessionBuilder();
    }

    public ParkingSessionBuilder WithId(Guid id)
    {
        this.id = id;
        return this;
    }

    public ParkingSessionBuilder WithUser(User user)
    {
        this.user = user;
        return this;
    }

    public ParkingSessionBuilder WithStartDate(DateTime startDate)
    {
        this.startDate = startDate;
        return this;
    }

    public ParkingSessionBuilder WithEndDate(DateTime endDate)
    {
        this.endDate = endDate;
        return this;
    }

    public ParkingSessionBuilder WithStatus(ParkingSessionStatus status)
    {
        this.status = status;
        return this;
    }

    public ParkingSession Build()
    {
        return new ParkingSession()
        {
            Id = this.id,
            UserId = this.user.Id,
            User = this.user,
            StartDate = this.startDate,
            EndDate = this.endDate,
            Status = this.status,
        };
    }
}
