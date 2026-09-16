<template>
  <main class="container page">
    <div v-if="move">
      <div class="d-flex flex-wrap align-items-center gap-3">
        <h1>{{ title }}</h1>
        <PokemonTypeImage :type="move.type" height="32" />
        <MoveCategoryBadge :category="move.category" height="32" />
      </div>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("moves.created.lead", { name: title }) }}</strong> {{ t("moves.created.help") }}
      </TarAlert>
      <StatusDetail class="mb-3" :subject="move" />
      <form class="border-top border-secondary-subtle pt-4" @submit.prevent="handleSubmit(submit)">
        <KeyAlreadyUsed v-model="keyAlreadyUsed" />
        <div class="row">
          <div class="col-md-6">
            <NameField class="mb-3" v-model="name" />
          </div>
          <div class="col-md-6">
            <KeyField class="mb-3" ref="keyField" required v-model="key" />
          </div>
        </div>
        <div class="row">
          <div class="col-md-4">
            <AccuracyField class="mb-3" v-model="accuracy" />
          </div>
          <div class="col-md-4">
            <PowerField class="mb-3" :disabled="move.category === 'Status'" v-model="power" />
          </div>
          <div class="col-md-4">
            <PowerPointsField class="mb-3" v-model="powerPoints" />
          </div>
        </div>
        <SummaryField class="mb-3" v-model="summary" />
        <ContentField class="mb-3" v-model="content" />
        <div class="d-flex justify-content-end mb-3">
          <TarButton
            :disabled="!hasChanges || isLoading"
            icon="fas fa-floppy-disk"
            :loading="isLoading"
            size="large"
            :status="t('loading')"
            :text="t('actions.save')"
            type="submit"
          />
        </div>
      </form>
    </div>
    <LoadingSpinner v-else />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AccuracyField from "@/components/moves/AccuracyField.vue";
import ContentField from "@/components/shared/ContentField.vue";
import KeyAlreadyUsed from "@/components/shared/KeyAlreadyUsed.vue";
import KeyField from "@/components/shared/KeyField.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import MoveCategoryBadge from "@/components/moves/MoveCategoryBadge.vue";
import NameField from "@/components/shared/NameField.vue";
import PokemonTypeImage from "@/components/pokemon/PokemonTypeImage.vue";
import PowerField from "@/components/moves/PowerField.vue";
import PowerPointsField from "@/components/moves/PowerPointsField.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarButton from "@/components/tar/TarButton.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { ApiFailure, ProblemDetails } from "@/types/api";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { Move, UpdateMovePayload } from "@/types/moves";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readMove, updateMove } from "@/api/moves";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";
import { useForm } from "@/forms";
import { useToastStore } from "@/stores/toast";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const accuracy = ref<number>(0);
const content = ref<string>("");
const isCreated = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const key = ref<string>("");
const keyAlreadyUsed = ref<boolean>(false);
const keyField = ref<InstanceType<typeof KeyField> | null>(null);
const move = ref<Move>();
const name = ref<string>("");
const power = ref<number>(0);
const powerPoints = ref<number>(0);
const summary = ref<string>("");

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("moves.title"), to: { name: "Moves" } }));
const hasChanges = computed<boolean>(() =>
  Boolean(
    move.value &&
    (move.value.key !== key.value ||
      (move.value.name ?? "") !== name.value ||
      (move.value.summary ?? "") !== summary.value ||
      (move.value.content ?? "") !== content.value ||
      (move.value.accuracy ?? 0) !== accuracy.value ||
      (move.value.power ?? 0) !== power.value ||
      (move.value.powerPoints ?? 0) !== powerPoints.value),
  ),
);
const title = computed<string>(() => move.value?.name ?? move.value?.key ?? "");

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && move.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    try {
      const payload: UpdateMovePayload = {
        key: key.value,
        name: { value: name.value },
        summary: { value: summary.value },
        content: { value: content.value },
        accuracy: { value: accuracy.value || null },
        power: { value: power.value || null },
        powerPoints: { value: powerPoints.value || null },
      };
      move.value = await updateMove(move.value.id, payload);
      isCreated.value = false;
      reinitialize();
      toasts.success("saved");
    } catch (e: unknown) {
      const failure = e as ApiFailure;
      if (failure.status === StatusCodes.Conflict) {
        const problemDetails = failure.data as ProblemDetails;
        if (problemDetails.error && problemDetails.error.code === ErrorCodes.KeyAlreadyUsed) {
          keyAlreadyUsed.value = true;
          keyField.value?.focus();
          return;
        }
      }
      handleError(e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  move,
  (move) => {
    key.value = move?.key ?? "";
    name.value = move?.name ?? "";
    summary.value = move?.summary ?? "";
    content.value = move?.content ?? "";
    accuracy.value = move?.accuracy ?? 0;
    power.value = move?.power ?? 0;
    powerPoints.value = move?.powerPoints ?? 0;
  },
  { deep: true },
);

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
