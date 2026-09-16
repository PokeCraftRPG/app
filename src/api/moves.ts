import { urlUtils } from "logitar-js";

import type { CreateOrReplaceMovePayload, Move, SearchMovesPayload, UpdateMovePayload } from "@/types/moves";
import type { SearchResults } from "@/types/search";
import { encodeSortOption } from "@/utils/search";
import { get, patch, post, put } from ".";

export async function createMove(payload: CreateOrReplaceMovePayload): Promise<Move> {
  const url: string = new urlUtils.UrlBuilder({ path: "/moves" }).buildRelative();
  return (await post<CreateOrReplaceMovePayload, Move>(url, payload)).data;
}

export async function readMove(id: string): Promise<Move> {
  const url: string = new urlUtils.UrlBuilder({ path: "/moves/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Move>(url)).data;
}

export async function replaceMove(id: string, payload: CreateOrReplaceMovePayload): Promise<Move> {
  const url: string = new urlUtils.UrlBuilder({ path: "/moves/{id}" }).setParameter("id", id).buildRelative();
  return (await put<CreateOrReplaceMovePayload, Move>(url, payload)).data;
}

export async function searchMoves(payload: SearchMovesPayload): Promise<SearchResults<Move>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/moves" })
    .setQuery("category", payload.category ?? "")
    .setQuery("ids", payload.ids)
    .setQuery("search", payload.search.terms)
    .setQuery("search_mode", payload.search.mode)
    .setQuery("type", payload.type ?? "")
    .setQuery("sort", payload.sort.map(encodeSortOption))
    .setQuery("offset", payload.offset.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<Move>>(url)).data;
}

export async function updateMove(id: string, payload: UpdateMovePayload): Promise<Move> {
  const url: string = new urlUtils.UrlBuilder({ path: "/moves/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateMovePayload, Move>(url, payload)).data;
}
