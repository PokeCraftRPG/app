using Logitar.EventSourcing;
using MediatR;

namespace PokeCraft.Domain.Speciez.Events;

public record SpeciesCreated(
  SpeciesNumber Number,
  SpeciesCategory Category,
  UniqueName UniqueName,
  Friendship BaseFriendship,
  CatchRate CatchRate,
  GrowthRate GrowthRate) : DomainEvent, INotification;
