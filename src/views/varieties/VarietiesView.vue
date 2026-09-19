<template>
  <main class="container page">
    <div v-if="filters" class="d-flex flex-column flex-grow-1">
      <div class="d-flex flex-column flex-sm-row justify-content-between align-items-sm-start gap-3">
        <h1 class="mb-0">{{ title }}</h1>
        <RouterLink class="btn btn-lg btn-primary" :to="{ name: 'VarietyCreate' }">
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
          <div class="col-md-4">
            <BooleanSelect class="mb-3" id="default" label="varieties.default.is" :model-value="isDefault" @update:model-value="setQuery('default', $event)" />
          </div>
          <div class="col-md-4">
            <BooleanSelect
              class="mb-3"
              id="metamorph"
              label="varieties.canChangeForm"
              :model-value="canChangeForm"
              @update:model-value="setQuery('metamorph', $event)"
            />
          </div>
          <div class="col-md-4">
            <SpeciesSelect class="mb-3" :model-value="species" :species="filters.species" @update:model-value="setQuery('species', $event)" />
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
          <div v-for="entry in varieties" :key="entry.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
            <VarietyLinkCard class="h-100" :variety="entry" />
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

import BooleanSelect from "@/components/shared/BooleanSelect.vue";
import ClearFiltersButton from "@/components/shared/ClearFiltersButton.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SearchPagination from "@/components/shared/SearchPagination.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import SpeciesSelect from "@/components/species/SpeciesSelect.vue";
import VarietyLinkCard from "@/components/varieties/VarietyLinkCard.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { SearchResults, SortDirection } from "@/types/search";
import type { SelectOption } from "@/types/tar/select";
import type { SearchVarietiesPayload, Variety, VarietyFilters, VarietySort } from "@/types/varieties";
import { handleErrorKey } from "@/inject";
import { parseTextSearch } from "@/utils/search";
import { getVarietyFilters, searchVarieties } from "@/api/varieties";
import { useDocument } from "@/composables/document";

const document = useDocument();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const filters = ref<VarietyFilters>();
const isLoading = ref<boolean>(false);
const timestamp = ref<number>(0);
const total = ref<number>(0);
const varieties = ref<Variety[]>([]);

const canChangeForm = computed<boolean | undefined>(() => parseBoolean(route.query.metamorph?.toString() ?? ""));
const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 12);
const direction = computed<string>(() => route.query.direction?.toString() ?? "");
const isDefault = computed<boolean | undefined>(() => parseBoolean(route.query.default?.toString() ?? ""));
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const species = computed<string>(() => route.query.species?.toString() ?? "");
const title = computed<string>(() => t("varieties.title"));

const hasFilters = computed<boolean>(() =>
  Boolean(typeof canChangeForm.value === "boolean" || typeof isDefault.value === "boolean" || search.value || species.value),
);

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("varieties.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

function clearFilters(): void {
  const query = { ...route.query, default: "", metamorph: "", search: "", species: "", page: 1 };
  router.replace({ ...route, query });
}

function setQuery(key: string, value?: boolean | null | number | string): void {
  const query = { ...route.query, [key]: value?.toString() ?? "" };
  switch (key) {
    case "default":
    case "metamorph":
    case "search":
    case "species":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

async function loadFilters(): Promise<void> {
  try {
    filters.value = await getVarietyFilters();
  } catch (e: unknown) {
    handleError(e);
  }

  if (species.value && !filters.value?.species.some((entry) => entry.id === species.value)) {
    setQuery("species", "");
  }
}
async function loadResults(): Promise<void> {
  const payload: SearchVarietiesPayload = {
    canChangeForm: canChangeForm.value,
    ids: [],
    isDefault: isDefault.value,
    search: parseTextSearch(search.value),
    sort: sort.value ? [{ field: sort.value as VarietySort, direction: direction.value as SortDirection }] : [],
    species: species.value || undefined,
    offset: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<Variety> = await searchVarieties(payload);
    if (now === timestamp.value) {
      varieties.value = [...results.items];
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
    if (route.name === "Varieties") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                default: "",
                metamorph: "",
                search: "",
                species: "",
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
