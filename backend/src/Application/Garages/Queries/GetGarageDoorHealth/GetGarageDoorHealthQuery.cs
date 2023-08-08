using System.Text.Json.Serialization;
using Domain.Entities;
using MediatR;

namespace Application.Garages.Queries.GetGarageDoorHealth;

public record GetGarageDoorHealthQuery(Guid GarageId, Guid DoorId) : IRequest<GetGarageDoorHealthResponse>;

public class GetGarageDoorHealthResponse
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public DoorHealth Health { get; set; }
}
