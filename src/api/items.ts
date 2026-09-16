import { urlUtils } from "logitar-js";

import type { CreateOrReplaceItemPayload, Item, SearchItemsPayload, UpdateItemPayload } from "@/types/items";
import type { SearchResults } from "@/types/search";
import { encodeSortOption } from "@/utils/search";
import { get, patch, post, put } from ".";

export async function createItem(payload: CreateOrReplaceItemPayload): Promise<Item> {
  const url: string = new urlUtils.UrlBuilder({ path: "/items" }).buildRelative();
  return (await post<CreateOrReplaceItemPayload, Item>(url, payload)).data;
}

export async function readItem(id: string): Promise<Item> {
  const url: string = new urlUtils.UrlBuilder({ path: "/items/{id}" }).setParameter("id", id).buildRelative();
  return (await get<Item>(url)).data;
}

export async function replaceItem(id: string, payload: CreateOrReplaceItemPayload): Promise<Item> {
  const url: string = new urlUtils.UrlBuilder({ path: "/items/{id}" }).setParameter("id", id).buildRelative();
  return (await put<CreateOrReplaceItemPayload, Item>(url, payload)).data;
}

export async function searchItems(payload: SearchItemsPayload): Promise<SearchResults<Item>> {
  const url: string = new urlUtils.UrlBuilder({ path: "/items" })
    .setQuery("category", payload.category ?? "")
    .setQuery("ids", payload.ids)
    .setQuery("search", payload.search.terms)
    .setQuery("search_mode", payload.search.mode)
    .setQuery("sort", payload.sort.map(encodeSortOption))
    .setQuery("offset", payload.offset.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<Item>>(url)).data;
}

export async function updateItem(id: string, payload: UpdateItemPayload): Promise<Item> {
  const url: string = new urlUtils.UrlBuilder({ path: "/items/{id}" }).setParameter("id", id).buildRelative();
  return (await patch<UpdateItemPayload, Item>(url, payload)).data;
}
