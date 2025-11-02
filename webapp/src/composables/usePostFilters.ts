import { ref } from "vue";

export type SortType = "popular" | "nearby" | "recent";
export type FilterType = "recreational" | "infrastructure" | "all";

// Shared state across all components
const currentSort = ref<SortType>("recent");
const currentFilter = ref<FilterType>("all");

export function usePostFilters() {
  const setSort = (sort: SortType) => {
    currentSort.value = sort;
  };

  const setFilter = (filter: FilterType) => {
    currentFilter.value = filter;
  };

  const reset = () => {
    currentSort.value = "recent";
    currentFilter.value = "all";
  };

  return {
    currentSort,
    currentFilter,
    setSort,
    setFilter,
    reset,
  };
}
