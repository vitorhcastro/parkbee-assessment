using Domain.Entities;
using FluentValidation;

namespace Application.Users.Queries.GetUserParkingSessionsById;

public class GetUserParkingSessionsByIdQueryValidator : AbstractValidator<GetUserParkingSessionsByIdQuery>
{
    public GetUserParkingSessionsByIdQueryValidator()
    {
        this.RuleFor(v => v.ParkingSessionStatus)
            .Must(v => v == null || Enum.IsDefined(typeof(ParkingSessionStatus), v))
            .WithMessage(v => $"Invalid enum value {v.ParkingSessionStatus} for {nameof(v.ParkingSessionStatus)}");
    }
}
