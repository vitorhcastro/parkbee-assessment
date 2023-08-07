namespace Api.Tests.Integration.Steps;

public class CreateParkingSessionRequest
{
    public Guid UserId { get; set; }
    public Guid GarageId { get; set; }
    public Guid DoorId { get; set; }
}
