import { urlUtils } from "logitar-js";

import type { CreateOrReplaceRegionPayload, Region, SearchRegionsPayload, UpdateRegionPayload } from "@/types/regions";
import type { SearchResults } from "@/types/search";
import { encodeSortOption } from "@/utils/search";
import { get, patch, post, put } from ".";

export async function createRegion(payload: CreateOrReplaceRegionPayload): Promise<Region> {
  const url: string = new urlUtils.UrlBuilder({ path: "/regions" }).buildRelative();
  return (await post<CreateOrReplaceRegionPayload, Region>(url, payload)).data;
}

export async function readRegion(id: string): Promise<Region> {
  const url: string = new urlUtils.UrlBuilder({ path: "/regions/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Region>(url)).data;
}

export async function replaceRegion(id: string, payload: CreateOrReplaceRegionPayload): Promise<Region> {
  const url: string = new urlUtils.UrlBuilder({ path: "/regions/{id}" }).setParameter("id", id).buildRelative();
  return (await put<CreateOrReplaceRegionPayload, Region>(url, payload)).data;
}

export async function searchRegions(payload: SearchRegionsPayload): Promise<SearchResults<Region>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/regions" })
    .setQuery("ids", payload.ids)
    .setQuery("search", payload.search.terms)
    .setQuery("search_mode", payload.search.mode)
    .setQuery("sort", payload.sort.map(encodeSortOption))
    .setQuery("offset", payload.offset.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<Region>>(url)).data;
}

export async function updateRegion(id: string, payload: UpdateRegionPayload): Promise<Region> {
  const url: string = new urlUtils.UrlBuilder({ path: "/regions/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateRegionPayload, Region>(url, payload)).data;
}
