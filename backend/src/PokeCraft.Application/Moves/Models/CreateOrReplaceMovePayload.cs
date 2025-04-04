﻿using PokeCraft.Domain;
using PokeCraft.Domain.Moves;

namespace PokeCraft.Application.Moves.Models;

public record CreateOrReplaceMovePayload
{
  public PokemonType Type { get; set; }
  public MoveCategory Category { get; set; }

  public string UniqueName { get; set; } = string.Empty;
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public int? Accuracy { get; set; }
  public int? Power { get; set; }
  public int PowerPoints { get; set; }

  public InflictedStatusModel? InflictedStatus { get; set; }
  public List<StatisticChangeModel> StatisticChanges { get; set; } = [];
  public List<string> VolatileConditions { get; set; } = [];

  public string? Link { get; set; }
  public string? Notes { get; set; }
}
