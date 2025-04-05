using Logitar.EventSourcing;
using MediatR;

namespace PokeCraft.Domain.Worlds.Events;

public record WorldUpdated : DomainEvent, INotification
{
  public Slug? UniqueSlug { get; set; }
  public Change<DisplayName>? DisplayName { get; set; }
  public Change<Description>? Description { get; set; }
}
