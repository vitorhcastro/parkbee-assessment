using System.Text.Json.Serialization;

namespace Domain.Entities;

public class Door
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DoorType DoorType { get; set; }
    public Guid GarageId { get; set; }
    public string IpAddress { get; set; }
}
