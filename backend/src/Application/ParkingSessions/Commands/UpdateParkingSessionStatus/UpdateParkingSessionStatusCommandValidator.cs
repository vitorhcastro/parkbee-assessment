using Domain.Entities;
using FluentValidation;

namespace Application.ParkingSessions.Commands.UpdateParkingSessionStatus;

public class UpdateParkingSessionStatusCommandValidator : AbstractValidator<UpdateParkingSessionStatusCommand>
{
    public UpdateParkingSessionStatusCommandValidator()
    {
        this.RuleFor(v => v.Request.Status)
            .Must(v => Enum.IsDefined(typeof(ParkingSessionStatus), v))
            .WithMessage(v => $"Invalid enum value {v.Request.Status} for {nameof(v.Request.Status)}");
    }
}
