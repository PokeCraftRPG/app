import { urlUtils } from "logitar-js";

import type {
  CreateOrReplaceVarietyPayload,
  SearchVarietiesPayload,
  SetVarietyMovePayload,
  UpdateVarietyPayload,
  Variety,
  VarietyFilters,
} from "@/types/varieties";
import type { SearchResults } from "@/types/search";
import { _delete, get, patch, post, put } from ".";
import { encodeSortOption } from "@/utils/search";

export async function addVarietyMove(varietyId: string, payload: SetVarietyMovePayload): Promise<Variety> {
  const url: string = new urlUtils.UrlBuilder({ path: "/varieties/{varietyId}/moves" }).setParameter("varietyId", varietyId).buildRelative();
  return (await post<SetVarietyMovePayload, Variety>(url, payload)).data;
}

export async function createVariety(payload: CreateOrReplaceVarietyPayload): Promise<Variety> {
  const url: string = new urlUtils.UrlBuilder({ path: "/varieties" }).buildRelative();
  return (await post<CreateOrReplaceVarietyPayload, Variety>(url, payload)).data;
}

export async function getVarietyFilters(): Promise<VarietyFilters> {
  const url: string = new urlUtils.UrlBuilder({ path: "/varieties/filters" }).buildRelative();
  return (await get<VarietyFilters>(url)).data;
}

export async function readVariety(id: string): Promise<Variety> {
  const url: string = new urlUtils.UrlBuilder({ path: "/varieties/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Variety>(url)).data;
}

export async function removeVarietyMove(varietyId: string, id: string): Promise<Variety> {
  const url: string = new urlUtils.UrlBuilder({ path: "/varieties/{varietyId}/moves/{id}" })
    .setParameter("varietyId", varietyId)
    .setParameter("id", id)
    .buildRelative();
  return (await _delete<Variety>(url)).data;
}

export async function searchVarieties(payload: SearchVarietiesPayload): Promise<SearchResults<Variety>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/varieties" })
    .setQuery("default", payload.isDefault?.toString() ?? "")
    .setQuery("ids", payload.ids)
    .setQuery("metamorph", payload.canChangeForm?.toString() ?? "")
    .setQuery("search", payload.search.terms)
    .setQuery("search_mode", payload.search.mode)
    .setQuery("species", payload.species ?? "")
    .setQuery("sort", payload.sort.map(encodeSortOption))
    .setQuery("offset", payload.offset.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<Variety>>(url)).data;
}

export async function setVarietyMove(varietyId: string, id: string, payload: SetVarietyMovePayload): Promise<Variety> {
  const url: string = new urlUtils.UrlBuilder({ path: "/varieties/{varietyId}/moves/{id}" })
    .setParameter("varietyId", varietyId)
    .setParameter("id", id)
    .buildRelative();
  return (await put<SetVarietyMovePayload, Variety>(url, payload)).data;
}

export async function updateVariety(id: string, payload: UpdateVarietyPayload): Promise<Variety> {
  const url: string = new urlUtils.UrlBuilder({ path: "/varieties/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateVarietyPayload, Variety>(url, payload)).data;
}
