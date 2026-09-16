import type { Actor, Aggregate, Optional } from "./api";
import type { SearchPayload } from "./search";

export type CreateOrReplaceWorldPayload = {
  key: string;
  name?: string | null;
  summary?: string | null;
  content?: string | null;
};

export type Member = {
  user: Actor;
  grantedBy: Actor;
  grantedOn: string;
};

export type SearchWorldsPayload = SearchPayload<WorldSort>;

export type UpdateWorldPayload = {
  key?: string | null;
  name?: Optional<string> | null;
  summary?: Optional<string> | null;
  content?: Optional<string> | null;
};

export type World = Aggregate & {
  key: string;
  name?: string | null;
  summary?: string | null;
  content?: string | null;
  owner: Actor;
  members: Member[];
};

export type WorldSort = "CreatedOn" | "Key" | "Name" | "UpdatedOn";
