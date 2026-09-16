import type { Aggregate, Optional } from "./api";
import type { SearchPayload, SortOption } from "./search";

export type Ability = Aggregate & {
  key: string;
  name?: string | null;
  summary?: string | null;
  content?: string | null;
};

export type AbilitySort = "CreatedOn" | "Key" | "Name" | "UpdatedOn";

export type AbilitySortOption = SortOption & {
  field: AbilitySort;
};

export type CreateOrReplaceAbilityPayload = {
  key: string;
  name?: string | null;
  summary?: string | null;
  content?: string | null;
};

export type SearchAbilitiesPayload = SearchPayload & {
  sort: AbilitySortOption[];
};

export type UpdateAbilityPayload = {
  key?: string | null;
  name?: Optional<string> | null;
  summary?: Optional<string> | null;
  content?: Optional<string> | null;
};
