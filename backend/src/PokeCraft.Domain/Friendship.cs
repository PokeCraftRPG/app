using FluentValidation;

namespace PokeCraft.Domain;

public record Friendship
{
  public const int MinimumValue = 0;
  public const int MaximumValue = 255;

  public int Value { get; }

  public Friendship() : this(MinimumValue)
  {
  }

  public Friendship(int value)
  {
    Value = value;
    new Validator().ValidateAndThrow(this);
  }

  public override string ToString() => Value.ToString();

  private class Validator : AbstractValidator<Friendship>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Friendship();
    }
  }
}
