import type { Aggregate, Optional } from "./api";
import type { SearchPayload } from "./search";

export type CreateOrReplaceRegionPayload = {
  key: string;
  name?: string | null;
  summary?: string | null;
  content?: string | null;
};

export type Region = Aggregate & {
  key: string;
  name?: string | null;
  summary?: string | null;
  content?: string | null;
};

export type RegionSort = "CreatedOn" | "Key" | "Name" | "UpdatedOn";

export type RegionSummary = {
  id: string;
  key: string;
  name?: string | null;
};

export type SearchRegionsPayload = SearchPayload<RegionSort>;

export type UpdateRegionPayload = {
  key?: string | null;
  name?: Optional<string> | null;
  summary?: Optional<string> | null;
  content?: Optional<string> | null;
};
