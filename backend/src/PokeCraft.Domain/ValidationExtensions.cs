using FluentValidation;
using PokeCraft.Domain.Validators;

namespace PokeCraft.Domain;

public static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, int> Accuracy<T>(this IRuleBuilder<T, int> ruleBuilder)
  {
    return ruleBuilder.InclusiveBetween(Moves.Accuracy.MinimumValue, Moves.Accuracy.MaximumValue);
  }

  public static IRuleBuilderOptions<T, string> AllowedCharacters<T>(this IRuleBuilder<T, string> ruleBuilder, string? allowedCharacters)
  {
    return ruleBuilder.SetValidator(new AllowedCharactersValidator<T>(allowedCharacters));
  }

  public static IRuleBuilderOptions<T, int> CatchRate<T>(this IRuleBuilder<T, int> ruleBuilder)
  {
    return ruleBuilder.InclusiveBetween(Speciez.CatchRate.MinimumValue, Speciez.CatchRate.MaximumValue);
  }

  public static IRuleBuilderOptions<T, string> Description<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty();
  }

  public static IRuleBuilderOptions<T, string> DisplayName<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Domain.DisplayName.MaximumLength);
  }

  public static IRuleBuilderOptions<T, int> Friendship<T>(this IRuleBuilder<T, int> ruleBuilder)
  {
    return ruleBuilder.InclusiveBetween(Domain.Friendship.MinimumValue, Domain.Friendship.MaximumValue);
  }

  public static IRuleBuilderOptions<T, string> Notes<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty();
  }

  public static IRuleBuilderOptions<T, int> Power<T>(this IRuleBuilder<T, int> ruleBuilder)
  {
    return ruleBuilder.InclusiveBetween(Moves.Power.MinimumValue, Moves.Power.MaximumValue);
  }

  public static IRuleBuilderOptions<T, int> PowerPoints<T>(this IRuleBuilder<T, int> ruleBuilder)
  {
    return ruleBuilder.InclusiveBetween(Moves.PowerPoints.MinimumValue, Moves.PowerPoints.MaximumValue);
  }

  public static IRuleBuilderOptions<T, string> Slug<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Domain.Slug.MaximumLength).SetValidator(new SlugValidator<T>());
  }

  public static IRuleBuilderOptions<T, int> SpeciesNumber<T>(this IRuleBuilder<T, int> ruleBuilder)
  {
    return ruleBuilder.InclusiveBetween(Speciez.SpeciesNumber.MinimumValue, Speciez.SpeciesNumber.MaximumValue);
  }

  public static IRuleBuilderOptions<T, string> UniqueName<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Domain.UniqueName.MaximumLength)
      .AllowedCharacters("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+");
  }

  public static IRuleBuilderOptions<T, string> Url<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Domain.Url.MaximumLength).SetValidator(new UrlValidator<T>());
  }

  public static IRuleBuilderOptions<T, string> VolatileCondition<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Moves.VolatileCondition.MaximumLength);
  }
}
