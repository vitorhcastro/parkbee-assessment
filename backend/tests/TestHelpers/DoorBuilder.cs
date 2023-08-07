using Domain.Entities;

namespace TestHelpers;

public class DoorBuilder
{
    private Guid id = Guid.NewGuid();

    public static DoorBuilder ADoor()
    {
        return new DoorBuilder();
    }

    public DoorBuilder WithId(Guid id)
    {
        this.id = id;
        return this;
    }

    public Door Build()
    {
        return new Door()
        {
            Id = this.id,
        };
    }
}
