using FluentValidation;

namespace PokeCraft.Domain.Moves;

public record PowerPoints
{
  public const int MinimumValue = 1;
  public const int MaximumValue = 40;

  public int Value { get; }

  public PowerPoints(int value)
  {
    Value = value;
  }

  private class Validator : AbstractValidator<PowerPoints>
  {
    public Validator()
    {
      RuleFor(x => x.Value).PowerPoints();
    }
  }
}
