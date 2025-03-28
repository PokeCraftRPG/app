using Logitar.EventSourcing;
using MediatR;

namespace PokeCraft.Domain.Speciez.Events;

public record SpeciesUpdated : DomainEvent, INotification
{
  public UniqueName? UniqueName { get; set; }
  public Change<DisplayName>? DisplayName { get; set; }

  public Friendship? BaseFriendship { get; set; }
  public CatchRate? CatchRate { get; set; }
  public GrowthRate? GrowthRate { get; set; }

  public Change<Url>? Link { get; set; }
  public Change<Notes>? Notes { get; set; }
}
