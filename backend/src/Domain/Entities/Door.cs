namespace Domain.Entities;

public class Door
{
    public Guid Id { get; set; }

    public string Description { get; set; }

    public DoorType DoorType { get; set; }

    public Guid GarageId { get; set; }
    public Garage Garage { get; set; }
}
