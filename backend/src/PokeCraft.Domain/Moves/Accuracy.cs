using FluentValidation;

namespace PokeCraft.Domain.Moves;

public record Accuracy
{
  public const int MinimumValue = 30;
  public const int MaximumValue = 100;

  public int Value { get; }

  public Accuracy(int value)
  {
    Value = value;
  }

  public static Accuracy? TryCreate(int? value) => value.HasValue ? new(value.Value) : null;

  private class Validator : AbstractValidator<Accuracy>
  {
    public Validator()
    {
      RuleFor(x => x.Value).Accuracy();
    }
  }
}
