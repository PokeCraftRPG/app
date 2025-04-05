using Logitar.EventSourcing;
using MediatR;

namespace PokeCraft.Domain.Regions.Events;

public record RegionUpdated : DomainEvent, INotification
{
  public UniqueName? UniqueName { get; set; }
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }

  public Change<Url>? Link { get; set; }
  public Change<Notes>? Notes { get; set; }
}
