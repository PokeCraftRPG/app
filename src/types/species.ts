import type { Aggregate, Optional } from "./api";
import type { FilterOption, SearchPayload } from "./search";

export type CreateOrReplaceSpeciesPayload = {
  number: number;
  category: SpeciesCategory;
  key: string;
  name?: string | null;
  summary?: string | null;
  content?: string | null;
  baseFriendship: number;
  catchRate: number;
  growthRate: GrowthRate;
  eggs: SpeciesEggs;
};

export type EggGroup =
  | "NoEggsDiscovered"
  | "Amorphous"
  | "Bug"
  | "Ditto"
  | "Dragon"
  | "Fairy"
  | "Field"
  | "Flying"
  | "Grass"
  | "HumanLike"
  | "Mineral"
  | "Monster"
  | "Water1"
  | "Water2"
  | "Water3";

export type GrowthRate = "Fluctuating" | "Slow" | "MediumSlow" | "MediumFast" | "Fast" | "Erratic";

export type SearchSpeciesPayload = SearchPayload<SpeciesSort> & {
  category?: SpeciesCategory | null;
  eggGroup?: EggGroup | null;
  growthRate?: GrowthRate | null;
  region?: string | null;
};

export type Species = Aggregate & {
  number: number;
  category: SpeciesCategory;
  key: string;
  name?: string | null;
  summary?: string | null;
  content?: string | null;
  baseFriendship: number;
  catchRate: number;
  growthRate: GrowthRate;
  eggs: SpeciesEggs;
};

export type SpeciesCategory = "Standard" | "Baby" | "Legendary" | "Mythical";

export type SpeciesEggs = {
  cycles: number;
  primaryGroup: EggGroup;
  secondaryGroup?: EggGroup | null;
};

export type SpeciesFilters = {
  regions: FilterOption[];
};

export type SpeciesSort = "BaseFriendship" | "CatchRate" | "CreatedOn" | "EggCycles" | "Key" | "Name" | "Number" | "UpdatedOn";

export type UpdateSpeciesPayload = {
  key?: string | null;
  name?: Optional<string> | null;
  summary?: Optional<string> | null;
  content?: Optional<string> | null;
  baseFriendship?: number | null;
  catchRate?: number | null;
  growthRate?: GrowthRate | null;
  eggs?: SpeciesEggs | null;
};
