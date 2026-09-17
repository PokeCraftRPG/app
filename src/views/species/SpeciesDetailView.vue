<template>
  <main class="container page">
    <div v-if="species">
      <div class="d-flex flex-wrap align-items-center gap-3">
        <h1>{{ title }}</h1>
        <TarBadge class="fs-6" variant="secondary">{{ t(`species.category.options.${species.category}`) }}</TarBadge>
      </div>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("species.created.lead", { name: title }) }}</strong> {{ t("species.created.help") }}
      </TarAlert>
      <StatusDetail class="mb-3" :subject="species" />
      <TarTabs :key="species.id" class="border-top border-secondary-subtle pt-4">
        <TarTab active id="properties" :title="t('properties')">
          <SpeciesProperties :species="species" @error="handleError" @updated="onUpdated" />
        </TarTab>
        <TarTab id="regional-numbers" :title="t('species.regionalNumbers.title')">
          <RegionalNumbers :species="species" @error="handleError" @updated="onUpdated" />
        </TarTab>
      </TarTabs>
    </div>
    <LoadingSpinner v-else />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import RegionalNumbers from "@/components/species/RegionalNumbers.vue";
import SpeciesProperties from "@/components/species/SpeciesProperties.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarBadge from "@/components/tar/TarBadge.vue";
import TarTab from "@/components/tar/TarTab.vue";
import TarTabs from "@/components/tar/TarTabs.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { ApiFailure } from "@/types/api";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { Species } from "@/types/species";
import { StatusCodes } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readSpecies } from "@/api/species";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";
import { useToastStore } from "@/stores/toast";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { n, t } = useI18n();

const isCreated = ref<boolean>(false);
const species = ref<Species>();

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("species.title"), to: { name: "Species" } }));
const title = computed<string>(() =>
  species.value ? [`#${n(species.value.number, "pokemonNumber")}`, species.value.name ?? species.value.key].join(" ") : "",
);

function onUpdated(updated: Species): void {
  species.value = updated;
  isCreated.value = false;
  document.setTitle(title.value);
  toasts.success("saved");
}

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    species.value = await readSpecies(id);
    isCreated.value = events.shift() === "created";
    document.setTitle(title.value);
  } catch (e: unknown) {
    const failure = e as ApiFailure;
    if (failure.status === StatusCodes.NotFound) {
      router.push("/not-found");
    } else {
      handleError(e);
    }
  }
});
</script>
