using MediatR;
using Microsoft.Extensions.Logging;
using PokeCraft.Domain.Storages.Events;

namespace PokeCraft.Infrastructure.Handlers;

internal class StorageEvents : INotificationHandler<ResourceStored>, INotificationHandler<StorageInitialized> // TODO(fpion): storage
{
  private readonly PokemonContext _context;
  private readonly ILogger<StorageEvents> _logger;

  public StorageEvents(PokemonContext context, ILogger<StorageEvents> logger)
  {
    _context = context;
    _logger = logger;
  }

  public Task Handle(ResourceStored @event, CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }

  public Task Handle(StorageInitialized @event, CancellationToken cancellationToken)
  {
    return Task.CompletedTask;
  }
}
