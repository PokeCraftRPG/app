<template>
  <main class="container page">
    <div v-if="region">
      <h1>{{ title }}</h1>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("regions.created.lead", { name: title }) }}</strong> {{ t("regions.created.help") }}
      </TarAlert>
      <StatusDetail class="mb-3" :subject="region" />
      <RegionProperties :region="region" @error="handleError" @updated="onUpdated" />
    </div>
    <LoadingSpinner v-else />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import RegionProperties from "@/components/regions/RegionProperties.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { ApiFailure } from "@/types/api";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { Region } from "@/types/regions";
import { StatusCodes } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readRegion } from "@/api/regions";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";
import { useToastStore } from "@/stores/toast";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const isCreated = ref<boolean>(false);
const region = ref<Region>();

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("regions.title"), to: { name: "Regions" } }));
const title = computed<string>(() => region.value?.name ?? region.value?.key ?? "");

function onUpdated(saved: Region): void {
  region.value = saved;
  isCreated.value = false;
  toasts.success("saved");
}

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    region.value = await readRegion(id);
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
