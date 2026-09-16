import { urlUtils } from "logitar-js";

import type { CreateOrReplaceTrainerPayload, SearchTrainersPayload, Trainer, UpdateTrainerPayload } from "@/types/trainers";
import type { SearchResults } from "@/types/search";
import { encodeSortOption } from "@/utils/search";
import { get, patch, post, put } from ".";

export async function createTrainer(payload: CreateOrReplaceTrainerPayload): Promise<Trainer> {
  const url: string = new urlUtils.UrlBuilder({ path: "/trainers" }).buildRelative();
  return (await post<CreateOrReplaceTrainerPayload, Trainer>(url, payload)).data;
}

export async function readTrainer(id: string): Promise<Trainer> {
  const url: string = new urlUtils.UrlBuilder({ path: "/trainers/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Trainer>(url)).data;
}

export async function replaceTrainer(id: string, payload: CreateOrReplaceTrainerPayload): Promise<Trainer> {
  const url: string = new urlUtils.UrlBuilder({ path: "/trainers/{id}" }).setParameter("id", id).buildRelative();
  return (await put<CreateOrReplaceTrainerPayload, Trainer>(url, payload)).data;
}

export async function searchTrainers(payload: SearchTrainersPayload): Promise<SearchResults<Trainer>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/trainers" })
    .setQuery("gender", payload.gender ?? "")
    .setQuery("ids", payload.ids)
    .setQuery("member", payload.memberId ?? "")
    .setQuery("search", payload.search.terms)
    .setQuery("search_mode", payload.search.mode)
    .setQuery("sort", payload.sort.map(encodeSortOption))
    .setQuery("offset", payload.offset.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<Trainer>>(url)).data;
}

export async function updateTrainer(id: string, payload: UpdateTrainerPayload): Promise<Trainer> {
  const url: string = new urlUtils.UrlBuilder({ path: "/trainers/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateTrainerPayload, Trainer>(url, payload)).data;
}
