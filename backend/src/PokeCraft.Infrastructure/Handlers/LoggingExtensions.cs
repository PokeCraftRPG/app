using Logitar;
using Logitar.EventSourcing;
using Microsoft.Extensions.Logging;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Infrastructure.Handlers;

internal static class LoggingExtensions
{
  public static void AlreadyExists<T>(this ILogger<T> logger, DomainEvent @event, long version)
  {
    logger.LogWarning("[{Timestamp}] event.id='{Id}', event.type='{Type}', reason='{Reason}'; The entity was not expected to exist but was found at version {Version}.",
      DateTime.Now.ToISOString(), @event.Id, @event.GetType(), nameof(AlreadyExists), version);
  }

  public static void NotFound<T>(this ILogger<T> logger, DomainEvent @event)
  {
    logger.LogWarning("[{Timestamp}] event.id='{Id}', event.type='{Type}', reason='{Reason}'; The entity was expected to be at version {Version} but was not found.",
      DateTime.Now.ToISOString(), @event.Id, @event.GetType(), nameof(NotFound), @event.Version - 1);
  }

  public static void Success<T>(this ILogger<T> logger, DomainEvent @event)
  {
    logger.LogInformation("[{Timestamp}] event.id='{Id}', event.type='{Type}'; The event was handled successfully.",
      DateTime.Now.ToISOString(), @event.Id, @event.GetType());
  }

  public static void UnexpectedVersion<T>(this ILogger<T> logger, DomainEvent @event, long version)
  {
    logger.LogWarning("[{Timestamp}] event.id='{Id}', event.type='{Type}', reason='{Reason}'; The entity was expected to be at version {Expected} but was found at version {Version}.",
      DateTime.Now.ToISOString(), @event.Id, @event.GetType(), nameof(UnexpectedVersion), @event.Version - 1, version);
  }

  public static void WorldNotFound<T>(this ILogger<T> logger, DomainEvent @event, WorldId worldId)
  {
    logger.LogWarning("[{Timestamp}] event.id='{Id}', event.type='{Type}', reason='{Reason}'; The world entity 'StreamId={StreamId}' was expected to exist but was not found.",
      DateTime.Now.ToISOString(), @event.Id, @event.GetType(), nameof(WorldNotFound), worldId);
  }
}
