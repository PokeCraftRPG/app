using Logitar.EventSourcing;
using MediatR;

namespace PokeCraft.Domain.Regions.Events;

public record RegionDeleted : DomainEvent, IDeleteEvent, INotification;
