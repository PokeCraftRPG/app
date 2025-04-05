using Logitar.EventSourcing;
using MediatR;

namespace PokeCraft.Domain.Storages.Events;

public record StorageInitialized(UserId UserId, long AllocatedBytes) : DomainEvent, INotification;
