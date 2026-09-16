import { urlUtils } from "logitar-js";

import type { Ability, CreateOrReplaceAbilityPayload, SearchAbilitiesPayload, UpdateAbilityPayload } from "@/types/abilities";
import type { SearchResults } from "@/types/search";
import { encodeSortOption } from "@/utils/search";
import { get, patch, post, put } from ".";

export async function createAbility(payload: CreateOrReplaceAbilityPayload): Promise<Ability> {
  const url: string = new urlUtils.UrlBuilder({ path: "/abilities" }).buildRelative();
  return (await post<CreateOrReplaceAbilityPayload, Ability>(url, payload)).data;
}

export async function readAbility(id: string): Promise<Ability> {
  const url: string = new urlUtils.UrlBuilder({ path: "/abilities/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Ability>(url)).data;
}

export async function replaceAbility(id: string, payload: CreateOrReplaceAbilityPayload): Promise<Ability> {
  const url: string = new urlUtils.UrlBuilder({ path: "/abilities/{id}" }).setParameter("id", id).buildRelative();
  return (await put<CreateOrReplaceAbilityPayload, Ability>(url, payload)).data;
}

export async function searchAbilities(payload: SearchAbilitiesPayload): Promise<SearchResults<Ability>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/abilities" })
    .setQuery("ids", payload.ids)
    .setQuery("search", payload.search.terms)
    .setQuery("search_mode", payload.search.mode)
    .setQuery("sort", payload.sort.map(encodeSortOption))
    .setQuery("offset", payload.offset.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<Ability>>(url)).data;
}

export async function updateAbility(id: string, payload: UpdateAbilityPayload): Promise<Ability> {
  const url: string = new urlUtils.UrlBuilder({ path: "/abilities/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateAbilityPayload, Ability>(url, payload)).data;
}
