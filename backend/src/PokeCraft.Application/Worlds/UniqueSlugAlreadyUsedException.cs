using Logitar;
using Logitar.Portal.Contracts;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Application.Worlds;

public class UniqueSlugAlreadyUsedException : ConflictException
{
  private const string ErrorMessage = "The specified unique slug is already used.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public Guid ConflictId
  {
    get => (Guid)Data[nameof(ConflictId)]!;
    private set => Data[nameof(ConflictId)] = value;
  }
  public string UniqueSlug
  {
    get => (string)Data[nameof(UniqueSlug)]!;
    private set => Data[nameof(UniqueSlug)] = value;
  }
  public string PropertyName
  {
    get => (string)Data[nameof(PropertyName)]!;
    private set => Data[nameof(PropertyName)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(WorldId)] = WorldId;
      error.Data[nameof(ConflictId)] = ConflictId;
      error.Data[nameof(UniqueSlug)] = UniqueSlug;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public UniqueSlugAlreadyUsedException(World world, WorldId conflictId) : base(BuildMessage(world, conflictId))
  {
    WorldId = world.Id.ToGuid();
    ConflictId = conflictId.ToGuid();
    UniqueSlug = world.UniqueSlug.Value;
    PropertyName = nameof(world.UniqueSlug);
  }

  private static string BuildMessage(World world, WorldId conflictId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), world.Id.ToGuid())
    .AddData(nameof(ConflictId), conflictId.ToGuid())
    .AddData(nameof(UniqueSlug), world.UniqueSlug)
    .AddData(nameof(PropertyName), nameof(world.UniqueSlug))
    .Build();
}
