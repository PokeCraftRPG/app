using Logitar.EventSourcing;
using MediatR;

namespace PokeCraft.Domain.Worlds.Events;

public record WorldDeleted : DomainEvent, IDeleteEvent, INotification;
