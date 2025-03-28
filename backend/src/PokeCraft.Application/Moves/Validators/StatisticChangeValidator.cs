using FluentValidation;
using PokeCraft.Application.Moves.Models;
using PokeCraft.Domain;
using PokeCraft.Domain.Moves;

namespace PokeCraft.Application.Moves.Validators;

internal class StatisticChangeValidator : AbstractValidator<StatisticChangeModel>
{
  public StatisticChangeValidator()
  {
    RuleFor(x => x.Statistic).IsInEnum().NotEqual(PokemonStatistic.HP);
    RuleFor(x => x.Stages).InclusiveBetween(Move.MinimumStage, Move.MaximumStage);
  }
}
