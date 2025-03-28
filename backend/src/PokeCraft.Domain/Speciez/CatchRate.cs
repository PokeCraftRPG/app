using FluentValidation;

namespace PokeCraft.Domain.Speciez;

public record CatchRate
{
  public const int MinimumValue = 1;
  public const int MaximumValue = 255;

  public int Value { get; }

  public CatchRate() : this(MinimumValue)
  {
  }

  public CatchRate(int value)
  {
    Value = value;
    new Validator().ValidateAndThrow(this);
  }

  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<CatchRate>
  {
    public Validator()
    {
      RuleFor(x => x.Value).CatchRate();
    }
  }
}
