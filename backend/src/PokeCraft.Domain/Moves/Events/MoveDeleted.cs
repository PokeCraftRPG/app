using Logitar.EventSourcing;
using MediatR;

namespace PokeCraft.Domain.Moves.Events;

public record MoveDeleted : DomainEvent, IDeleteEvent, INotification;
