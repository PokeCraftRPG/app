<template>
  <main class="container page">
    <div v-if="filters" class="d-flex flex-column flex-grow-1">
      <div class="d-flex flex-column flex-sm-row justify-content-between align-items-sm-start gap-3">
        <h1 class="mb-0">{{ title }}</h1>
        <RouterLink class="btn btn-lg btn-primary" :to="{ name: 'FormCreate' }">
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
            <VarietySelect class="mb-3" :model-value="variety" :varieties="filters.varieties" @update:model-value="setQuery('variety', $event)" />
          </div>
          <div class="col-md-6 col-lg-3">
            <FormCategorySelect class="mb-3" :model-value="category" @update:model-value="setQuery('category', $event)" />
          </div>
          <div class="col-md-6 col-lg-3">
            <PokemonTypeSelect class="mb-3" :model-value="type" @update:model-value="setQuery('type', $event)" />
          </div>
          <div class="col-md-6 col-lg-3">
            <AbilitySelect class="mb-3" :abilities="filters.abilities" :model-value="ability" @update:model-value="setQuery('ability', $event)" />
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
          <div v-for="entry in forms" :key="entry.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
            <FormLinkCard class="h-100" :form="entry" />
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

import AbilitySelect from "@/components/abilities/AbilitySelect.vue";
import ClearFiltersButton from "@/components/shared/ClearFiltersButton.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import FormCategorySelect from "@/components/pokemonForms/FormCategorySelect.vue";
import FormLinkCard from "@/components/pokemonForms/FormLinkCard.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import PokemonTypeSelect from "@/components/pokemon/PokemonTypeSelect.vue";
import RefreshButton from "@/components/shared/RefreshButton.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SearchPagination from "@/components/shared/SearchPagination.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import VarietySelect from "@/components/varieties/VarietySelect.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Form, FormCategory, FormFilters, FormSort, SearchFormsPayload } from "@/types/pokemonForms";
import type { PokemonType } from "@/types/pokemon";
import type { SearchResults, SortDirection } from "@/types/search";
import type { SelectOption } from "@/types/tar/select";
import { handleErrorKey } from "@/inject";
import { parseTextSearch } from "@/utils/search";
import { getFormFilters, searchForms } from "@/api/pokemonForms";
import { useDocument } from "@/composables/document";

const document = useDocument();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const filters = ref<FormFilters>();
const forms = ref<Form[]>([]);
const isLoading = ref<boolean>(false);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const ability = computed<string>(() => route.query.ability?.toString() ?? "");
const category = computed<FormCategory | "">(() => (route.query.category?.toString() as FormCategory) ?? "");
const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 12);
const direction = computed<string>(() => route.query.direction?.toString() ?? "");
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const title = computed<string>(() => t("forms.title"));
const type = computed<PokemonType | "">(() => (route.query.type?.toString() as PokemonType) ?? "");
const variety = computed<string>(() => route.query.variety?.toString() ?? "");

const hasFilters = computed<boolean>(() => Boolean(ability.value || category.value || search.value || type.value || variety.value));

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("forms.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

function clearFilters(): void {
  const query = { ...route.query, ability: "", category: "", search: "", type: "", variety: "", page: 1 };
  router.replace({ ...route, query });
}

function setQuery(key: string, value?: boolean | null | number | string): void {
  const query = { ...route.query, [key]: value?.toString() ?? "" };
  switch (key) {
    case "ability":
    case "category":
    case "search":
    case "type":
    case "variety":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

async function loadFilters(): Promise<void> {
  try {
    filters.value = await getFormFilters();
  } catch (e: unknown) {
    handleError(e);
  }

  if (ability.value && !filters.value?.abilities.some((entry) => entry.id === ability.value)) {
    setQuery("ability", "");
  }
  if (variety.value && !filters.value?.varieties.some((entry) => entry.id === variety.value)) {
    setQuery("variety", "");
  }
}
async function loadResults(): Promise<void> {
  const payload: SearchFormsPayload = {
    ability: ability.value || undefined,
    category: category.value || undefined,
    ids: [],
    search: parseTextSearch(search.value),
    sort: sort.value ? [{ field: sort.value as FormSort, direction: direction.value as SortDirection }] : [],
    type: type.value || undefined,
    variety: variety.value || undefined,
    offset: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results: SearchResults<Form> = await searchForms(payload);
    if (now === timestamp.value) {
      forms.value = [...results.items];
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
    if (route.name === "Forms") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                ability: "",
                category: "",
                search: "",
                type: "",
                variety: "",
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
