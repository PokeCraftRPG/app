import type { Ability, AbilitySummary } from "./abilities";
import type { Aggregate, Optional } from "./api";
import type { PokemonType } from "./pokemon";
import type { SearchPayload } from "./search";
import type { Variety, VarietySummary } from "./varieties";

export type BaseStatistics = {
  hp: number;
  attack: number;
  defense: number;
  specialAttack: number;
  specialDefense: number;
  speed: number;
};

export type CreateOrReplaceFormPayload = {
  varietyId: string;
  category: FormCategory;
  key: string;
  name?: string | null;
  summary?: string | null;
  content?: string | null;
  types: FormTypes;
  abilities: FormAbilitiesPayload;
  baseStatistics: BaseStatistics;
  yield: FormYield;
  size: FormSize;
};

export type Form = Aggregate & {
  variety: Variety;
  category: FormCategory;
  key: string;
  name?: string | null;
  summary?: string | null;
  content?: string | null;
  types: FormTypes;
  abilities: FormAbilities;
  baseStatistics: BaseStatistics;
  yield: FormYield;
  size: FormSize;
};

export type FormAbilities = {
  primary: Ability;
  secondary?: Ability | null;
  hidden?: Ability | null;
};

export type FormAbilitiesPayload = {
  primaryId: string;
  secondaryId?: string | null;
  hiddenId?: string | null;
};

export type FormCategory = "Default" | "Alternative" | "BattleOnly" | "Mega";

export type FormFilters = {
  varieties: VarietySummary[];
  abilities: AbilitySummary[];
};

export type FormSize = {
  height: number;
  weight: number;
};

export type FormSort = "CreatedOn" | "ExperienceYield" | "Height" | "Key" | "Name" | "UpdatedOn" | "Weight";

export type FormTypes = {
  primary: PokemonType;
  secondary?: PokemonType | null;
};

export type FormYield = {
  experience: number;
  hp: number;
  attack: number;
  defense: number;
  specialAttack: number;
  specialDefense: number;
  speed: number;
};

export type SearchFormsPayload = SearchPayload<FormSort> & {
  variety?: string | null;
  category?: FormCategory | null;
  type?: PokemonType | null;
  ability?: string | null;
};

export type UpdateFormPayload = {
  key?: string | null;
  name?: Optional<string> | null;
  summary?: Optional<string> | null;
  content?: Optional<string> | null;
};
