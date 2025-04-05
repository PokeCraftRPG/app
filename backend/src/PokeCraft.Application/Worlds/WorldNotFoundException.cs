using Logitar;
using PokeCraft.Domain;

namespace PokeCraft.Application.Worlds;

public class WorldNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified world could not be found.";

  public string World
  {
    get => (string)Data[nameof(World)]!;
    private set => Data[nameof(World)] = value;
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
      error.Data[nameof(World)] = World;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public WorldNotFoundException(string world, string propertyName) : base(BuildMessage(world, propertyName))
  {
    World = world;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string world, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(World), world)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
