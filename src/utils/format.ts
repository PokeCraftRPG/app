import type { Ability, AbilitySummary } from "@/types/abilities";
import type { Item } from "@/types/items";
import type { Move, MoveSummary } from "@/types/moves";
import type { Region, RegionSummary } from "@/types/regions";
import type { Species, SpeciesSummary } from "@/types/species";
import type { Trainer } from "@/types/trainers";
import type { Variety, VarietySummary } from "@/types/varieties";
import type { Form } from "@/types/pokemonForms";
import type { World } from "@/types/worlds";

export function formatAbility(ability: Ability | AbilitySummary): string {
  return ability.name ?? ability.key;
}

export function formatForm(form: Form): string {
  return form.name ?? form.key;
}

export function formatItem(item: Item): string {
  return item.name ?? item.key;
}

export function formatMove(move: Move | MoveSummary): string {
  return move.name ?? move.key;
}

export function formatPokemonNumber(number: number, n: (value: number, format: string) => string): string {
  return `#${n(number, "pokemonNumber")}`;
}

export function formatRegion(region: Region | RegionSummary): string {
  return region.name ?? region.key;
}

export function formatSpecies(species: Species | SpeciesSummary, n: (value: number, format: string) => string): string {
  const formatted: string = species.name ?? species.key;
  return species.number ? `${formatPokemonNumber(species.number, n)} ${formatted}` : formatted;
}

export function formatTrainer(trainer: Trainer): string {
  return trainer.name ?? trainer.key;
}

export function formatVariety(variety: Variety | VarietySummary): string {
  return variety.name ?? variety.key;
}

export function formatWorld(world: World): string {
  return world.name ?? world.key;
}
