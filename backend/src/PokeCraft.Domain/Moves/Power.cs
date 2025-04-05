using FluentValidation;

namespace PokeCraft.Domain.Moves;

public record Power
{
  public const int MinimumValue = 10;
  public const int MaximumValue = 250;

  public int Value { get; }

  public Power(int value)
  {
    Value = value;
  }

  public static Power? TryCreate(int? value) => value.HasValue ? new(value.Value) : null;

  private class Validator : AbstractValidator<Power>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Power();
    }
  }
}
