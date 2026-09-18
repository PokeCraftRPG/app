<template>
  <main class="container page">
    <div v-if="filters" class="d-flex flex-column flex-grow-1">
      <div class="d-flex flex-column flex-sm-row justify-content-between align-items-sm-start gap-3">
        <h1 class="mb-0">{{ title }}</h1>
        <RouterLink class="btn btn-lg btn-primary" :to="{ name: 'SpeciesCreate' }">
          <font-awesome-icon icon="fas fa-plus" aria-hidden="true" />&nbsp;{{ t("actions.create") }}
        </RouterLink>
      </div>
      <WorldBreadcrumb :current="title" />
      <section>
        <div class="d-flex gap-2 mb-3">
          <RefreshButton :loading="isLoading" @click="refresh" />
          <ClearFiltersButton v-if="hasFilters" @click="clearFilters" />
        </div>
      </section>
      <section>
        <div class="row">
          <div class="col-md-6 col-lg-3">
            <SpeciesCategorySelect class="mb-3" :model-value="category" @update:model-value="setQuery('category', $event)" />
          </div>
          <div class="col-md-6 col-lg-3">
            <EggGroupSelect class="mb-3" :model-value="eggGroup" @update:model-value="setQuery('egg', $event)" />
          </div>
          <div class="col-md-6 col-lg-3">
            <GrowthRateSelect class="mb-3" :model-value="growthRate" @update:model-value="setQuery('growth', $event)" />
          </div>
          <div class="col-md-6 col-lg-3">
            <RegionSelect class="mb-3" :model-value="region" :regions="filters.regions" @update:model-value="setQuery('region', $event)" />
          </div>
        </div>
        <div class="row">
          <div class="col-md-4">
            <SearchInput class="mb-3" :model-value="search" @update:model-value="setQuery('search', $event)" />
          </div>
          <div class="col-md-4">
            <SortSelect
              class="mb-3"
              :direction="direction"
              :model-value="sort"
              :options="sortOptions"
              @update:direction="setQuery('direction', $event)"
              @update:model-value="setQuery('sort', $event)"
            />
          </div>
          <div class="col-md-4">
            <CountSelect class="mb-3" :model-value="count" @update:model-value="setQuery('count', $event)" />
          </div>
        </div>
      </section>
      <section v-if="total" class="border-top border-secondary-subtle pt-4" :class="{ loading: isLoading }">
        <div class="row">
          <div v-for="entry in species" :key="entry.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
            <SpeciesLinkCard class="h-100" :species="entry" />
          </div>
        </div>
        <SearchPagination v-if="total > count" class="mt-3" :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event)" />
      </section>
      <section v-else class="d-flex flex-column align-items-center justify-content-center text-center flex-grow-1 py-5" :class="{ loading: isLoading }">
        <font-awesome-icon icon="fas fa-magnifying-glass" class="display-4 text-body-secondary mb-3" aria-hidden="true" />
        <h2 class="h4 mb-2">{{ t("empty.lead") }}</h2>
        <p class="text-body-secondary mb-0">{{ t("empty.help") }}</p>
        <ClearFiltersButton v-if="hasFilters" class="mt-3" @click="clearFilters" />
      </section>
    </div>
    <LoadingSpinner v-else />
  </main>
</template>

<script setup lang="ts">
import { arrayUtils, objectUtils, parsingUtils } from "logitar-js";
import { computed, inject, ref, watch, watchEffect } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import ClearFiltersButton from "@/components/shared/ClearFiltersButton.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import EggGroupSelect from "@/components/species/EggGroupSelect.vue";
import GrowthRateSelect from "@/components/species/GrowthRateSelect.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import RegionSelect from "@/components/regions/RegionSelect.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SearchPagination from "@/components/shared/SearchPagination.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import SpeciesCategorySelect from "@/components/species/SpeciesCategorySelect.vue";
import SpeciesLinkCard from "@/components/species/SpeciesLinkCard.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { EggGroup, GrowthRate, SearchSpeciesPayload, Species, SpeciesCategory, SpeciesFilters, SpeciesSort } from "@/types/species";
import type { SearchResults, SortDirection } from "@/types/search";
import type { SelectOption } from "@/types/tar/select";
import { handleErrorKey } from "@/inject";
import { parseTextSearch } from "@/utils/search";
import { getSpeciesFilters, searchSpecies } from "@/api/species";
import { useDocument } from "@/composables/document";

const document = useDocument();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const filters = ref<SpeciesFilters>();
const isLoading = ref<boolean>(false);
const species = ref<Species[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const category = computed<SpeciesCategory | "">(() => route.query.category?.toString() as SpeciesCategory | "");
const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 12);
const direction = computed<string>(() => route.query.direction?.toString() ?? "");
const eggGroup = computed<EggGroup | "">(() => route.query.egg?.toString() as EggGroup | "");
const growthRate = computed<GrowthRate | "">(() => route.query.growth?.toString() as GrowthRate | "");
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const region = computed<string>(() => route.query.region?.toString() ?? "");
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const title = computed<string>(() => t("species.title"));

const hasFilters = computed<boolean>(() => Boolean(category.value || eggGroup.value || growthRate.value || region.value || search.value));

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("species.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

function clearFilters(): void {
  const query = { ...route.query, category: "", egg: "", growth: "", region: "", search: "", page: 1 };
  router.replace({ ...route, query });
}

function setQuery(key: string, value?: boolean | null | number | string): void {
  const query = { ...route.query, [key]: value?.toString() ?? "" };
  switch (key) {
    case "category":
    case "egg":
    case "growth":
    case "region":
    case "search":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

async function loadFilters(): Promise<void> {
  try {
    filters.value = await getSpeciesFilters();
  } catch (e: unknown) {
    handleError(e);
  }

  if (region.value && !filters.value?.regions.some(({ id }) => id === region.value)) {
    setQuery("region", "");
  }
}
async function loadResults(): Promise<void> {
  const payload: SearchSpeciesPayload = {
    category: category.value ? (category.value as SpeciesCategory) : undefined,
    eggGroup: eggGroup.value ? (eggGroup.value as EggGroup) : undefined,
    growthRate: growthRate.value ? (growthRate.value as GrowthRate) : undefined,
    ids: [],
    region: region.value,
    search: parseTextSearch(search.value),
    sort: sort.value ? [{ field: sort.value as SpeciesSort, direction: direction.value as SortDirection }] : [],
    offset: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<Species> = await searchSpecies(payload);
    if (now === timestamp.value) {
      species.value = [...results.items];
      total.value = results.total;
    }
  } catch (e: unknown) {
    handleError(e);
  } finally {
    if (now === timestamp.value) {
      isLoading.value = false;
    }
  }
}
async function refresh(): Promise<void> {
  await loadFilters();
  await loadResults();
}

watch(
  () => route,
  (route) => {
    if (route.name === "Species") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                category: "",
                egg: "",
                growth: "",
                region: "",
                search: "",
                sort: "Name",
                direction: "Ascending",
                page: 1,
                count: 12,
              }
            : {
                page: 1,
                count: 12,
                ...query,
              },
        });
      } else if (!filters.value) {
        refresh();
      } else {
        loadResults();
      }
    }
  },
  { deep: true, immediate: true },
);

watchEffect(() => document.setTitle(title.value));
</script>
