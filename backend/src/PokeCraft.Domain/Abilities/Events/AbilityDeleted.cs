using Logitar.EventSourcing;
using MediatR;

namespace PokeCraft.Domain.Abilities.Events;

public record AbilityDeleted : DomainEvent, IDeleteEvent, INotification;
