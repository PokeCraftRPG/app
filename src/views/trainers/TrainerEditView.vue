<template>
  <main class="container page">
    <div v-if="trainer">
      <div class="d-flex justify-content-between align-items-start gap-4">
        <div class="flex-grow-1">
          <h1>{{ title }}</h1>
          <WorldBreadcrumb :current="title" :parent="breadcrumb" />
          <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
            <strong>{{ t("trainers.created.lead", { name: title }) }}</strong> {{ t("trainers.created.help") }}
          </TarAlert>
          <StatusDetail class="mb-3" :subject="trainer" />
        </div>
        <ImageAsset v-if="trainer.sprite" :alt="t('sprite.alt', { name: title })" :asset="trainer.sprite" height="144" />
      </div>
      <TrainerProperties :trainer="trainer" @error="handleError" @updated="onUpdated" />
    </div>
    <LoadingSpinner v-else />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import ImageAsset from "@/components/shared/ImageAsset.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TrainerProperties from "@/components/trainers/TrainerProperties.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { ApiFailure } from "@/types/api";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { Trainer } from "@/types/trainers";
import { StatusCodes } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readTrainer } from "@/api/trainers";
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
const trainer = ref<Trainer>();

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("trainers.title"), to: { name: "Trainers" } }));
const title = computed<string>(() => trainer.value?.name ?? trainer.value?.key ?? "");

function onUpdated(saved: Trainer): void {
  trainer.value = saved;
  isCreated.value = false;
  toasts.success("saved");
}

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    trainer.value = await readTrainer(id);
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
