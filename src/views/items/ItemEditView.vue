<template>
  <main class="container page">
    <div v-if="item">
      <div class="d-flex justify-content-between align-items-start gap-4">
        <div class="flex-grow-1">
          <h1>{{ title }}</h1>
          <WorldBreadcrumb :current="title" :parent="breadcrumb" />
          <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
            <strong>{{ t("items.created.lead", { name: title }) }}</strong> {{ t("items.created.help") }}
          </TarAlert>
          <StatusDetail class="mb-3" :subject="item" />
        </div>
        <ImageAsset v-if="item.sprite" :alt="t('sprite.alt', { name: title })" :asset="item.sprite" height="144" />
      </div>
      <ItemProperties :item="item" @error="handleError" @updated="onUpdated" />
    </div>
    <LoadingSpinner v-else />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import ImageAsset from "@/components/shared/ImageAsset.vue";
import ItemProperties from "@/components/items/ItemProperties.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { ApiFailure } from "@/types/api";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { Item } from "@/types/items";
import { StatusCodes } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readItem } from "@/api/items";
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
const item = ref<Item>();

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("items.title"), to: { name: "Items" } }));
const title = computed<string>(() => item.value?.name ?? item.value?.key ?? "");

function onUpdated(saved: Item): void {
  item.value = saved;
  isCreated.value = false;
  toasts.success("saved");
}

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    item.value = await readItem(id);
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
