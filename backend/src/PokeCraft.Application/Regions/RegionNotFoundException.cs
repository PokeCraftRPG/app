using Logitar;
using Logitar.Portal.Contracts;

namespace PokeCraft.Application.Regions;

public class RegionNotFoundException : NotFoundException
{
  private const string ErrorMessage = "The specified region could not be found.";

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
      error.Data[nameof(Region)] = Region;
      error.Data[nameof(PropertyName)] = PropertyName;
      return error;
    }
  }

  public RegionNotFoundException(string region, string propertyName) : base(BuildMessage(region, propertyName))
  {
    Region = region;
    PropertyName = propertyName;
  }

  private static string BuildMessage(string region, string propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Region), region)
    .AddData(nameof(PropertyName), propertyName)
    .Build();
}
