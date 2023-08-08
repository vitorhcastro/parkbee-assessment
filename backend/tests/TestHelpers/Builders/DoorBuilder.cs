using Domain.Entities;
using Infrastructure;
using static TestHelpers.Builders.GarageBuilder;

namespace TestHelpers.Builders;

public class DoorBuilder
{
    private Guid id = Guid.NewGuid();
    private Garage garage = AGarage().Build();
    private DoorType doorType = DoorType.Entry;
    private string ipAddress = ApplicationConstants.ParkbeeDotComIpAddress;

    public static DoorBuilder ADoor()
    {
        return new DoorBuilder();
    }

    public DoorBuilder WithId(Guid id)
    {
        this.id = id;
        return this;
    }

    public DoorBuilder WithGarage(Garage garage)
    {
        this.garage = garage;
        return this;
    }

    public DoorBuilder WithDoorType(DoorType doorType)
    {
        this.doorType = doorType;
        return this;
    }

    public DoorBuilder WithIpAddress(string ipAddress)
    {
        this.ipAddress = ipAddress;
        return this;
    }

    public Door Build()
    {
        return new Door()
        {
            Id = this.id,
            GarageId = this.garage.Id,
            DoorType = this.doorType,
            IpAddress = this.ipAddress,
        };
    }
}
