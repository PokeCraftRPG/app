import { urlUtils } from "logitar-js";

import type {
  CreateOrReplaceSpeciesPayload,
  SearchSpeciesPayload,
  SetRegionalNumberPayload,
  Species,
  SpeciesFilters,
  UpdateSpeciesPayload,
} from "@/types/species";
import type { SearchResults } from "@/types/search";
import { _delete, get, patch, post, put } from ".";
import { encodeSortOption } from "@/utils/search";

export async function createSpecies(payload: CreateOrReplaceSpeciesPayload): Promise<Species> {
  const url: string = new urlUtils.UrlBuilder({ path: "/species" }).buildRelative();
  return (await post<CreateOrReplaceSpeciesPayload, Species>(url, payload)).data;
}

export async function getSpeciesFilters(): Promise<SpeciesFilters> {
  const url: string = new urlUtils.UrlBuilder({ path: "/species/filters" }).buildRelative();
  return (await get<SpeciesFilters>(url)).data;
}

export async function readSpecies(id: string): Promise<Species> {
  const url: string = new urlUtils.UrlBuilder({ path: "/species/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Species>(url)).data;
}

export async function removeRegionalNumber(speciesId: string, regionId: string): Promise<Species> {
  const url: string = new urlUtils.UrlBuilder({ path: "/species/{speciesId}/regions/{regionId}" })
    .setParameter("speciesId", speciesId)
    .setParameter("regionId", regionId)
    .buildRelative();
  return (await _delete<Species>(url)).data;
}

export async function searchSpecies(payload: SearchSpeciesPayload): Promise<SearchResults<Species>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/species" })
    .setQuery("category", payload.category ?? "")
    .setQuery("egg", payload.eggGroup ?? "")
    .setQuery("growth", payload.growthRate ?? "")
    .setQuery("ids", payload.ids)
    .setQuery("region", payload.region ?? "")
    .setQuery("search", payload.search.terms)
    .setQuery("search_mode", payload.search.mode)
    .setQuery("sort", payload.sort.map(encodeSortOption))
    .setQuery("offset", payload.offset.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<Species>>(url)).data;
}

export async function setRegionalNumber(speciesId: string, regionId: string, payload: SetRegionalNumberPayload): Promise<Species> {
  const url: string = new urlUtils.UrlBuilder({ path: "/species/{speciesId}/regions/{regionId}" })
    .setParameter("speciesId", speciesId)
    .setParameter("regionId", regionId)
    .buildRelative();
  return (await put<SetRegionalNumberPayload, Species>(url, payload)).data;
}

export async function updateSpecies(id: string, payload: UpdateSpeciesPayload): Promise<Species> {
  const url: string = new urlUtils.UrlBuilder({ path: "/species/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateSpeciesPayload, Species>(url, payload)).data;
}
