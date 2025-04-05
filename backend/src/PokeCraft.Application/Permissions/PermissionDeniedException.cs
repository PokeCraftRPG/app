using Logitar;
using PokeCraft.Domain;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Application.Permissions;

public class PermissionDeniedException : ForbiddenException
{
  private const string ErrorMessage = "The specified action has been denied for the specified principal on the specified resource.";

  public string Principal
  {
    get => (string)Data[nameof(Principal)]!;
    set => Data[nameof(Principal)] = value;
  }
  public ActionKind Action
  {
    get => (ActionKind)Data[nameof(Action)]!;
    set => Data[nameof(Action)] = value;
  }
  public string Resource
  {
    get => (string)Data[nameof(Resource)]!;
    private set => Data[nameof(Resource)] = value;
  }
  public Guid? WorldId
  {
    get => (Guid?)Data[nameof(WorldId)];
    private set => Data[nameof(WorldId)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(Principal)] = Principal;
      error.Data[nameof(Action)] = Action;
      error.Data[nameof(Resource)] = Resource;
      error.Data[nameof(WorldId)] = WorldId;
      return error;
    }
  }

  public PermissionDeniedException(UserId userId, ActionKind action, ResourceType resourceType, WorldId? worldId = null)
    : this(userId, action, FormatResource(resourceType), worldId)
  {
  }
  public PermissionDeniedException(UserId userId, ActionKind action, IResource resource, WorldId? worldId = null)
    : this(userId, action, FormatResource(resource), worldId)
  {
  }
  private PermissionDeniedException(UserId userId, ActionKind action, string resource, WorldId? worldId)
    : base(BuildMessage(userId, action, resource, worldId))
  {
    Principal = FormatPrincipal(userId);
    Action = action;
    Resource = resource;
    WorldId = worldId?.ToGuid();
  }

  private static string BuildMessage(UserId userId, ActionKind action, string resource, WorldId? worldId) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Principal), FormatPrincipal(userId))
    .AddData(nameof(Action), action)
    .AddData(nameof(Resource), resource)
    .AddData(nameof(WorldId), worldId?.ToGuid(), "<null>")
    .Build();

  private static string FormatPrincipal(UserId userId) => $"UserId:{userId.ToGuid()}";

  private static string FormatResource(IResource resource) => $"{ResourceType.World}:{resource.WorldId.ToGuid()}|{resource.ResourceType}:{resource.EntityId}";
  private static string FormatResource(ResourceType resourceType) => $"{resourceType}:*";
}
