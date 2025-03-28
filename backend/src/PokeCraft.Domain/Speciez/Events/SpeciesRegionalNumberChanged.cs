using Logitar.EventSourcing;
using MediatR;
using PokeCraft.Domain.Regions;

namespace PokeCraft.Domain.Speciez.Events;

public record SpeciesRegionalNumberChanged(RegionId RegionId, SpeciesNumber? Number) : DomainEvent, INotification;
