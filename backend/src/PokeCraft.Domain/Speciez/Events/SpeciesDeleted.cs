using Logitar.EventSourcing;
using MediatR;

namespace PokeCraft.Domain.Speciez.Events;

public record SpeciesDeleted : DomainEvent, IDeleteEvent, INotification;
