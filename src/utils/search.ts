import type { TextSearch } from "@/types/search";

export function toSkip(page: number, count: number): number {
  if (page < 1 || count < 1) {
    return 0;
  }
  return (page - 1) * count;
}

export function toTextSearch(search: string): TextSearch {
  return {
    operator: "And",
    terms: search
      .split(" ")
      .filter((term) => term.length)
      .map((term) => ({ value: `%${term}%` })),
  };
}
