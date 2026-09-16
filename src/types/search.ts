export type FilterOption = {
  text: string;
  value: string;
};

export type SearchMode = "All" | "Any";

export type SearchPayload<T> = {
  ids: string[];
  search: TextSearch;
  sort: SortOption<T>[];
  offset: number;
  limit: number;
};

export type SearchResults<T> = {
  items: T[];
  total: number;
};

export type SortDirection = "Ascending" | "Descending";

export type SortOption<T> = {
  field: T;
  direction: SortDirection;
};

export type TextSearch = {
  terms: string[];
  mode: SearchMode;
};
