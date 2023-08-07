using Domain.Common;

namespace Api.Tests.Integration.Steps;

public class UpdateParkingSessionStatusRequest
{
    public ParkingSessionStatus Status { get; set; }
    public Guid ExitDoorId { get; set; }
}