namespace Domain.Entities;

public class ParkingSession
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public ParkingSessionStatus Status { get; set; }
    public Guid EntryDoorId { get; set; }
    public Door EntryDoor { get; set; }
    public Guid? ExitDoorId { get; set; }
    public Door? ExitDoor { get; set; }
}
