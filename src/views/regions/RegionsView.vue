<template>
  <main class="container page">
    <div v-if="hasLoaded" class="d-flex flex-column flex-grow-1">
      <div class="d-flex flex-column flex-sm-row justify-content-between align-items-sm-start gap-3">
        <h1 class="mb-0">{{ title }}</h1>
        <RouterLink class="btn btn-lg btn-primary" :to="{ name: 'RegionCreate' }">
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
          <div v-for="region in regions" :key="region.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
            <RegionLinkCard class="h-100" :region="region" />
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
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import RegionLinkCard from "@/components/regions/RegionLinkCard.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SearchPagination from "@/components/shared/SearchPagination.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Region, RegionSort, SearchRegionsPayload } from "@/types/regions";
import type { SearchResults, SortDirection } from "@/types/search";
import type { SelectOption } from "@/types/tar/select";
import { handleErrorKey } from "@/inject";
import { parseTextSearch } from "@/utils/search";
import { searchRegions } from "@/api/regions";
import { useDocument } from "@/composables/document";

const document = useDocument();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const hasLoaded = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const regions = ref<Region[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 12);
const direction = computed<string>(() => route.query.direction?.toString() ?? "");
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const title = computed<string>(() => t("regions.title"));

const hasFilters = computed<boolean>(() => Boolean(search.value));

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("regions.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

function clearFilters(): void {
  const query = { ...route.query, search: "", page: 1 };
  router.replace({ ...route, query });
}

function setQuery(key: string, value?: boolean | null | number | string): void {
  const query = { ...route.query, [key]: value?.toString() ?? "" };
  switch (key) {
    case "search":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

async function refresh(): Promise<void> {
  const payload: SearchRegionsPayload = {
    ids: [],
    search: parseTextSearch(search.value),
    sort: sort.value ? [{ field: sort.value as RegionSort, direction: direction.value as SortDirection }] : [],
    offset: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<Region> = await searchRegions(payload);
    if (now === timestamp.value) {
      regions.value = [...results.items];
      total.value = results.total;
    }
  } catch (e: unknown) {
    handleError(e);
  } finally {
    if (now === timestamp.value) {
      isLoading.value = false;
    }
    hasLoaded.value = true;
  }
}

watch(
  () => route,
  (route) => {
    if (route.name === "Regions") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
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
      } else {
        refresh();
      }
    }
  },
  { deep: true, immediate: true },
);

watchEffect(() => document.setTitle(title.value));
</script>
