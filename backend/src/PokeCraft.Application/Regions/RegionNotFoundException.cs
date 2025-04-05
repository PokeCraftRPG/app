using Logitar;
using PokeCraft.Domain;
using PokeCraft.Domain.Worlds;

namespace PokeCraft.Application.Regions;

public class RegionNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified region could not be found.";

  public Guid WorldId
  {
    get => (Guid)Data[nameof(WorldId)]!;
    private set => Data[nameof(WorldId)] = value;
  }
  public string Region
  {
    get => (string)Data[nameof(Region)]!;
    private set => Data[nameof(Region)] = value;
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
      error.Data[nameof(Region)] = Region;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public RegionNotFoundException(WorldId worldId, string region, string propertyName) : base(BuildMessage(worldId, region, propertyName))
  {
    WorldId = worldId.ToGuid();
    Region = region;
    PropertyName = propertyName;
  }

  private static string BuildMessage(WorldId worldId, string region, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(WorldId), worldId.ToGuid())
    .AddData(nameof(Region), region)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
