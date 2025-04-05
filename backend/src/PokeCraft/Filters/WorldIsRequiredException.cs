using Logitar;
using PokeCraft.Application;
using PokeCraft.Domain;

namespace PokeCraft.Filters;

public class WorldIsRequiredException : BadRequestException
{
  private const string ErrorMessage = "A world is required. Enter your world ID or unique slug in the specified header.";

  public string Header
  {
    get => (string)Data[nameof(Header)]!;
    private set => Data[nameof(Header)] = value;
  }

  public override Error Error
  {
    get
    {
      Error error = new(this.GetErrorCode(), ErrorMessage);
      error.Data[nameof(Header)] = Header;
      return error;
    }
  }

  public WorldIsRequiredException(string header) : base(BuildMessage(header))
  {
    Header = header;
  }

  private static string BuildMessage(string header) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Header), header)
    .Build();
}
