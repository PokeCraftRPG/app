import type { Aggregate, Auditable, Optional } from "./api";
import type { Move, MoveSummary } from "./moves";
import type { SearchPayload } from "./search";
import type { Species, SpeciesSummary } from "./species";

export type CreateOrReplaceVarietyPayload = {
  speciesId: string;
  isDefault: boolean;
  key: string;
  name?: string | null;
  summary?: string | null;
  content?: string | null;
  canChangeForm: boolean;
  genderRatio?: number | null;
  genus?: string | null;
};

export type LearningMethod = "LevelUp" | "Evolution" | "Reminder";

export type SearchVarietiesPayload = SearchPayload<VarietySort> & {
  canChangeForm?: boolean | null;
  isDefault?: boolean | null;
  species?: string | null;
};

export type SetVarietyMovePayload = {
  moveId: string;
  learningMethod: LearningMethod;
  level?: number | null;
};

export type UpdateVarietyPayload = {
  isDefault?: boolean | null;
  key?: string | null;
  name?: Optional<string> | null;
  summary?: Optional<string> | null;
  content?: Optional<string> | null;
  canChangeForm?: boolean | null;
  genderRatio?: Optional<number | null> | null;
  genus?: Optional<string> | null;
};

export type Variety = Aggregate & {
  species: Species;
  isDefault: boolean;
  key: string;
  name?: string | null;
  summary?: string | null;
  content?: string | null;
  canChangeForm: boolean;
  genderRatio?: number | null;
  genus?: string | null;
  moves: VarietyMove[];
};

export type VarietyFilters = {
  species: SpeciesSummary[];
  moves: MoveSummary[];
};

export type VarietyMove = Auditable & {
  id: string;
  move: Move;
  learningMethod: LearningMethod;
  level?: number | null;
};

export type VarietySort = "CreatedOn" | "Key" | "Name" | "UpdatedOn";

export type VarietySummary = {
  id: string;
  key: string;
  name?: string | null;
};
