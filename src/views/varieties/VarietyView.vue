<template>
  <main class="container page">
    <div v-if="variety">
      <div class="d-flex flex-wrap align-items-center gap-3">
        <h1>{{ title }}</h1>
        <TarBadge class="fs-6" variant="secondary">{{ formatSpecies(variety.species, n) }}</TarBadge>
        <DefaultBadge v-if="variety.isDefault" class="fs-6" />
      </div>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("varieties.created.lead", { name: title }) }}</strong> {{ t("varieties.created.help") }}
      </TarAlert>
      <StatusDetail class="mb-3" :subject="variety" />
      <TarTabs :key="variety.id" class="border-top border-secondary-subtle pt-4">
        <TarTab active id="properties" :title="t('properties')">
          <VarietyProperties :variety="variety" @error="handleError" @updated="onUpdated" />
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

import DefaultBadge from "@/components/varieties/DefaultBadge.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarBadge from "@/components/tar/TarBadge.vue";
import TarTab from "@/components/tar/TarTab.vue";
import TarTabs from "@/components/tar/TarTabs.vue";
import VarietyProperties from "@/components/varieties/VarietyProperties.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { ApiFailure } from "@/types/api";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { Variety } from "@/types/varieties";
import { StatusCodes } from "@/types/api";
import { formatSpecies, formatVariety } from "@/utils/format";
import { handleErrorKey } from "@/inject";
import { readVariety } from "@/api/varieties";
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
const variety = ref<Variety>();

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("varieties.title"), to: { name: "Varieties" } }));
const title = computed<string>(() => (variety.value ? formatVariety(variety.value) : ""));

function onUpdated(updated: Variety): void {
  variety.value = updated;
  isCreated.value = false;
  document.setTitle(title.value);
  toasts.success("saved");
}

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    variety.value = await readVariety(id);
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
