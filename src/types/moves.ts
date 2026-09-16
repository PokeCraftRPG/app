import type { Aggregate, Optional } from "./api";
import type { PokemonType } from "./pokemon";
import type { SearchPayload } from "./search";

export type CreateOrReplaceMovePayload = {
  type: PokemonType;
  category: MoveCategory;
  key: string;
  name?: string | null;
  summary?: string | null;
  content?: string | null;
  accuracy?: number | null;
  power?: number | null;
  powerPoints?: number | null;
};

export type Move = Aggregate & {
  type: PokemonType;
  category: MoveCategory;
  key: string;
  name?: string | null;
  summary?: string | null;
  content?: string | null;
  accuracy?: number | null;
  power?: number | null;
  powerPoints?: number | null;
};

export type MoveCategory = "Physical" | "Special" | "Status";

export type MoveSort = "Accuracy" | "CreatedOn" | "Key" | "Name" | "Power" | "PowerPoints" | "UpdatedOn";

export type SearchMovesPayload = SearchPayload<MoveSort> & {
  type?: PokemonType | null;
  category?: MoveCategory | null;
};

export type UpdateMovePayload = {
  key?: string | null;
  name?: Optional<string> | null;
  summary?: Optional<string> | null;
  content?: Optional<string> | null;
  accuracy?: Optional<number> | null;
  power?: Optional<number> | null;
  powerPoints?: Optional<number> | null;
};
