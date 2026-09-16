import type { Aggregate, Optional } from "./api";
import type { Asset } from "./assets";
import type { SearchPayload } from "./search";

export type CreateOrReplaceItemPayload = {
  category: ItemCategory;
  key: string;
  name?: string | null;
  summary?: string | null;
  content?: string | null;
  price?: number | null;
  weight?: number | null;
  spriteId?: string | null;
};

export type Item = Aggregate & {
  category: ItemCategory;
  key: string;
  name?: string | null;
  summary?: string | null;
  content?: string | null;
  price?: number | null;
  weight?: number | null;
  sprite?: Asset | null;
};

export type ItemCategory = "Battle" | "Berry" | "Key" | "Machine" | "Material" | "Medicine" | "Other" | "PokeBall" | "Treasure";

export type ItemSort = "CreatedOn" | "Key" | "Name" | "Price" | "Weight" | "UpdatedOn";

export type SearchItemsPayload = SearchPayload<ItemSort> & {
  category?: ItemCategory | null;
};

export type UpdateItemPayload = {
  key?: string | null;
  name?: Optional<string> | null;
  summary?: Optional<string> | null;
  content?: Optional<string> | null;
  price?: Optional<number> | null;
  weight?: Optional<number> | null;
  spriteId?: Optional<string> | null;
};
