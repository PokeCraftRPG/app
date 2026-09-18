<template>
  <main class="container page">
    <div v-if="move">
      <h1>{{ title }}</h1>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("moves.created.lead", { name: title }) }}</strong> {{ t("moves.created.help") }}
      </TarAlert>
      <StatusDetail class="mb-3" :subject="move" />
      <MoveProperties :move="move" @error="handleError" @updated="onUpdated" />
    </div>
    <LoadingSpinner v-else />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import MoveProperties from "@/components/moves/MoveProperties.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { ApiFailure } from "@/types/api";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { Move } from "@/types/moves";
import { StatusCodes } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readMove } from "@/api/moves";
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
const move = ref<Move>();

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("moves.title"), to: { name: "Moves" } }));
const title = computed<string>(() => move.value?.name ?? move.value?.key ?? "");

function onUpdated(saved: Move): void {
  move.value = saved;
  isCreated.value = false;
  toasts.success("saved");
}

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    move.value = await readMove(id);
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
