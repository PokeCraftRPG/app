using Logitar.EventSourcing;
using MediatR;

namespace PokeCraft.Domain.Worlds.Events;

public record WorldCreated(UserId OwnerId, Slug UniqueSlug) : DomainEvent, INotification;
