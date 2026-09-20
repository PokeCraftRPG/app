import { urlUtils } from "logitar-js";

import type { CreateOrReplaceFormPayload, Form, FormFilters, SearchFormsPayload, UpdateFormPayload } from "@/types/pokemonForms";
import type { SearchResults } from "@/types/search";
import { encodeSortOption } from "@/utils/search";
import { get, patch, post } from ".";

export async function createForm(payload: CreateOrReplaceFormPayload): Promise<Form> {
  const url: string = new urlUtils.UrlBuilder({ path: "/forms" }).buildRelative();
  return (await post<CreateOrReplaceFormPayload, Form>(url, payload)).data;
}

export async function getFormFilters(): Promise<FormFilters> {
  const url: string = new urlUtils.UrlBuilder({ path: "/forms/filters" }).buildRelative();
  return (await get<FormFilters>(url)).data;
}

export async function readForm(id: string): Promise<Form> {
  const url: string = new urlUtils.UrlBuilder({ path: "/forms/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Form>(url)).data;
}

export async function searchForms(payload: SearchFormsPayload): Promise<SearchResults<Form>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/forms" })
    .setQuery("ability", payload.ability ?? "")
    .setQuery("category", payload.category ?? "")
    .setQuery("ids", payload.ids)
    .setQuery("search", payload.search.terms)
    .setQuery("search_mode", payload.search.mode)
    .setQuery("type", payload.type ?? "")
    .setQuery("variety", payload.variety ?? "")
    .setQuery("sort", payload.sort.map(encodeSortOption))
    .setQuery("offset", payload.offset.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<Form>>(url)).data;
}

export async function updateForm(id: string, payload: UpdateFormPayload): Promise<Form> {
  const url: string = new urlUtils.UrlBuilder({ path: "/forms/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateFormPayload, Form>(url, payload)).data;
}
