using FluentValidation;

namespace PokeCraft.Domain;

public record UniqueName
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }
  public int Size => Value.Length;

  public UniqueName(string value)
  {
    Value = value.Trim();
    new Validator().ValidateAndThrow(this);
  }

  public override string ToString() => Value;

  private class Validator : AbstractValidator<UniqueName>
  {
    public Validator()
    {
      RuleFor(x => x.Value).UniqueName();
    }
  }
}
