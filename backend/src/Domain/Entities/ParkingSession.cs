namespace Domain.Entities;

public class ParkingSession
{
    public Guid Id { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public ParkingSessionStatus Status { get; set; }
}
