﻿using Logitar.EventSourcing;
using Logitar.EventSourcing.Infrastructure;
using MediatR;

namespace PokeCraft.Infrastructure;

internal class EventBus : IEventBus
{
  private readonly IPublisher _publisher;

  public EventBus(IPublisher publisher)
  {
    _publisher = publisher;
  }

  public async Task PublishAsync(IEvent @event, CancellationToken cancellationToken)
  {
    await _publisher.Publish(@event, cancellationToken);
  }
}
