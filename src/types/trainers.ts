import type { Actor, Aggregate, Optional } from "./api";
import type { Asset } from "./assets";
import type { SearchPayload } from "./search";

export type CreateOrReplaceTrainerPayload = {
  key: string;
  name?: string | null;
  summary?: string | null;
  content?: string | null;
  license?: string | null;
  gender?: Gender | null;
  money: number;
  spriteId?: string | null;
  partyLimit?: number | null;
  memberId?: string | null;
};

export type Gender = "Male" | "Female";

export type SearchTrainersPayload = SearchPayload<TrainerSort> & {
  gender?: Gender | null;
  memberId?: string | null;
};

export type Trainer = Aggregate & {
  key: string;
  name?: string | null;
  summary?: string | null;
  content?: string | null;
  license?: string | null;
  gender?: Gender | null;
  money: number;
  sprite?: Asset | null;
  member?: Actor | null;
  partyCount: number;
  partyLimit?: number | null;
};

export type TrainerFilters = {
  members: Actor[];
};

export type TrainerSort = "CreatedOn" | "Key" | "License" | "Money" | "Name" | "UpdatedOn";

export type UpdateTrainerPayload = {
  key?: string | null;
  name?: Optional<string> | null;
  summary?: Optional<string> | null;
  content?: Optional<string> | null;
  license?: Optional<string> | null;
  gender?: Optional<Gender> | null;
  money?: number | null;
  spriteId?: Optional<string> | null;
  memberId?: Optional<string> | null;
  partyLimit?: Optional<number> | null;
};
