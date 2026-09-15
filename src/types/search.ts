export type SearchMode = "All" | "Any";

export type SearchPayload = {
  ids: string[];
  search: TextSearch;
  sort: SortOption[];
  offset: number;
  limit: number;
};

export type SearchResults<T> = {
  items: T[];
  total: number;
};

export type SearchTerm = {
  value: string;
};

export type SortOption = {
  field: string;
  isDescending: boolean;
};

export type TextSearch = {
  terms: SearchTerm[];
  operator: SearchMode;
};
