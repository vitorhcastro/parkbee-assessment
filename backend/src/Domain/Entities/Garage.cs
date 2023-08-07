namespace Domain.Entities;

public class Garage
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public List<Door> Doors { get; set; }

    public int TotalSpots { get; set; }
}
