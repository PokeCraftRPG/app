using Logitar.EventSourcing;
using MediatR;

namespace PokeCraft.Domain.Regions.Events;

public record RegionCreated(UniqueName UniqueName) : DomainEvent, INotification;
