using FluentValidation;

namespace PokeCraft.Domain.Speciez;

public record SpeciesNumber
{
  public const int MinimumValue = 1;
  public const int MaximumValue = 9999;

  public int Value { get; }

  public SpeciesNumber(int value)
  {
    Value = value;
    new Validator().ValidateAndThrow(this);
  }

  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<SpeciesNumber>
  {
    public Validator()
    {
      RuleFor(x => x.Value).SpeciesNumber();
    }
  }
}
