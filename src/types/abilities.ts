import type { Aggregate, Optional } from "./api";
import type { SearchPayload } from "./search";

export type Ability = Aggregate & {
  key: string;
  name?: string | null;
  summary?: string | null;
  content?: string | null;
};

export type AbilitySort = "CreatedOn" | "Key" | "Name" | "UpdatedOn";

export type CreateOrReplaceAbilityPayload = {
  key: string;
  name?: string | null;
  summary?: string | null;
  content?: string | null;
};

export type SearchAbilitiesPayload = SearchPayload<AbilitySort>;

export type UpdateAbilityPayload = {
  key?: string | null;
  name?: Optional<string> | null;
  summary?: Optional<string> | null;
  content?: Optional<string> | null;
};
