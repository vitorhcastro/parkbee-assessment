using System.Text.Json.Serialization;
using Domain.Entities;
using MediatR;

namespace Application.Garages.Queries.GetGarageDoorStatus;

public record GetGarageDoorStatusQuery(Guid GarageId, Guid DoorId) : IRequest<GetGarageDoorStatusResponse>;

public class GetGarageDoorStatusResponse
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DoorStatus Status { get; set; }
}
