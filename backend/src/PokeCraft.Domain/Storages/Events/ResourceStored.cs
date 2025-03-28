using Logitar.EventSourcing;
using MediatR;

namespace PokeCraft.Domain.Storages.Events;

public record ResourceStored(string Key, long Size) : DomainEvent, INotification;
