using Logitar.EventSourcing;
using MediatR;

namespace PokeCraft.Domain.Abilities.Events;

public record AbilityCreated(UniqueName UniqueName) : DomainEvent, INotification;
