using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Actors;
using Microsoft.EntityFrameworkCore;
using PokeCraft.Application;
using PokeCraft.Application.Moves;
using PokeCraft.Application.Moves.Models;
using PokeCraft.Application.Worlds.Models;
using PokeCraft.Domain;
using PokeCraft.Domain.Moves;
using PokeCraft.Infrastructure.Actors;
using PokeCraft.Infrastructure.Entities;
using PokeCraft.Infrastructure.PokemonDb;

namespace PokeCraft.Infrastructure.Queriers;

internal class MoveQuerier : IMoveQuerier
{
  private readonly IActorService _actorService;
  private readonly IApplicationContext _applicationContext;
  private readonly DbSet<MoveEntity> _moves;

  public MoveQuerier(IActorService actorService, IApplicationContext applicationContext, PokemonContext context)
  {
    _actorService = actorService;
    _applicationContext = applicationContext;
    _moves = context.Moves;
  }

  public async Task<MoveId?> FindIdAsync(UniqueName uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    string? streamId = await _moves.AsNoTracking()
      .WhereWorld(_applicationContext.WorldId)
      .Where(x => x.UniqueNameNormalized == uniqueNameNormalized)
      .Select(x => x.StreamId)
      .SingleOrDefaultAsync(cancellationToken);

    return streamId is null ? null : new MoveId(streamId);
  }

  public async Task<MoveModel> ReadAsync(Move move, CancellationToken cancellationToken)
  {
    return await ReadAsync(move.Id, cancellationToken)
      ?? throw new InvalidOperationException($"The move entity 'StreamId={move.Id}' could not be found.");
  }
  public async Task<MoveModel?> ReadAsync(MoveId id, CancellationToken cancellationToken)
  {
    string streamId = id.Value;

    MoveEntity? move = await _moves.AsNoTracking()
      .Include(x => x.World)
      .SingleOrDefaultAsync(x => x.StreamId == streamId, cancellationToken);

    return move is null ? null : await MapAsync(move, cancellationToken);
  }
  public async Task<MoveModel?> ReadAsync(Guid id, CancellationToken cancellationToken)
  {
    MoveEntity? move = await _moves.AsNoTracking()
      .WhereWorld(_applicationContext.WorldId)
      .SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    return move is null ? null : await MapAsync(move, cancellationToken);
  }
  public async Task<MoveModel?> ReadAsync(string uniqueName, CancellationToken cancellationToken)
  {
    string uniqueNameNormalized = Helper.Normalize(uniqueName);

    MoveEntity? move = await _moves.AsNoTracking()
      .WhereWorld(_applicationContext.WorldId)
      .SingleOrDefaultAsync(x => x.UniqueNameNormalized == uniqueNameNormalized, cancellationToken);

    return move is null ? null : await MapAsync(move, cancellationToken);
  }

  private async Task<MoveModel> MapAsync(MoveEntity move, CancellationToken cancellationToken)
  {
    return (await MapAsync([move], cancellationToken)).Single();
  }
  private async Task<IReadOnlyCollection<MoveModel>> MapAsync(IEnumerable<MoveEntity> moves, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = moves.SelectMany(move => move.GetActorIds());
    IReadOnlyCollection<ActorModel> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    WorldModel world = _applicationContext.World;
    return moves.Select(move => move.World is null ? mapper.ToMove(move, world) : mapper.ToMove(move)).ToList().AsReadOnly();
  }
}
