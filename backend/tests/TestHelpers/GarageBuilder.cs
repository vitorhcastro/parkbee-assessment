using Domain.Entities;
using Infrastructure;

namespace TestHelpers;

public class GarageBuilder
{
    private Guid id = Guid.NewGuid();
    private string name = Guid.NewGuid().ToString();
    private List<Door> doors = new();

    public static GarageBuilder AGarage()
    {
        return new GarageBuilder();
    }

    public GarageBuilder WithId(Guid id)
    {
        this.id = id;
        return this;
    }

    public GarageBuilder WithName(string name)
    {
        this.name = name;
        return this;
    }

    public GarageBuilder WithDoor(Door door)
    {
        this.doors.Add(door);
        return this;
    }

    public Garage Build()
    {
        return new Garage()
        {
            Id = this.id,
            Name = this.name,
            Doors = this.doors,
        };
    }
}
