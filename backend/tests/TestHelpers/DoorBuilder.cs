using Domain.Entities;
using static TestHelpers.GarageBuilder;

namespace TestHelpers;

public class DoorBuilder
{
    private Guid id = Guid.NewGuid();
    private Garage garage = AGarage().Build();
    private DoorType doorType = DoorType.Entry;

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

    public Door Build()
    {
        return new Door()
        {
            Id = this.id,
            GarageId = this.garage.Id,
            Garage = this.garage,
            DoorType = this.doorType,
        };
    }
}
