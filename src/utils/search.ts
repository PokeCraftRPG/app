import type { SortOption, TextSearch } from "@/types/search";

export function encodeSortOption<T>(option: SortOption<T>): string {
  let operator: string = "";
  switch (option.direction) {
    case "Ascending":
      operator = "+";
      break;
    case "Descending":
      operator = "-";
      break;
  }
  return operator + option.field;
}

export function parseTextSearch(search: string): TextSearch {
  const terms: Set<string> = new Set(
    search
      .split(" ")
      .filter((term) => term.length > 1 && term.length <= 100)
      .slice(0, 10),
  );
  return { terms: [...terms.values()], mode: "All" };
}
