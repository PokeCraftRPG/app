<template>
  <form class="border-top border-secondary-subtle pt-4" @submit.prevent="handleSubmit(submit)">
    <KeyAlreadyUsed v-model="keyAlreadyUsed" />
    <div class="row">
      <div class="col-md-6">
        <PokemonTypeField class="mb-3" :disabled="Boolean(move)" required v-model="type" />
      </div>
      <div class="col-md-6">
        <MoveCategoryField class="mb-3" :disabled="Boolean(move)" required v-model="category" />
      </div>
    </div>
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
        <PowerField class="mb-3" :disabled="category === 'Status'" v-model="power" />
      </div>
      <div class="col-md-4">
        <PowerPointsField class="mb-3" v-model="powerPoints" />
      </div>
    </div>
    <SummaryField class="mb-3" v-model="summary" />
    <ContentField class="mb-3" v-model="content" />
    <div class="d-flex justify-content-end mb-3">
      <SaveButton
        :disabled="!hasChanges || isLoading"
        :icon="move ? 'fas fa-floppy-disk' : 'fas fa-plus'"
        :loading="isLoading"
        :text="move ? 'actions.save' : 'actions.create'"
      />
    </div>
  </form>
</template>

<script setup lang="ts">
import { computed, nextTick, ref, watch } from "vue";
import { stringUtils } from "logitar-js";

import AccuracyField from "@/components/moves/AccuracyField.vue";
import ContentField from "@/components/shared/ContentField.vue";
import KeyAlreadyUsed from "@/components/shared/KeyAlreadyUsed.vue";
import KeyField from "@/components/shared/KeyField.vue";
import MoveCategoryField from "./MoveCategoryField.vue";
import NameField from "@/components/shared/NameField.vue";
import PokemonTypeField from "@/components/pokemon/PokemonTypeField.vue";
import PowerField from "@/components/moves/PowerField.vue";
import PowerPointsField from "@/components/moves/PowerPointsField.vue";
import SaveButton from "@/components/shared/SaveButton.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import type { ApiFailure, ProblemDetails } from "@/types/api";
import type { CreateOrReplaceMovePayload, Move, MoveCategory, UpdateMovePayload } from "@/types/moves";
import type { PokemonType } from "@/types/pokemon";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createMove, updateMove } from "@/api/moves";
import { useForm } from "@/forms";

const { slugify } = stringUtils;

const props = defineProps<{
  move?: Move;
}>();

const emit = defineEmits<{
  (e: "created", value: Move): void;
  (e: "error", value: unknown): void;
  (e: "updated", value: Move): void;
}>();

const accuracy = ref<number>();
const category = ref<MoveCategory | "">("");
const content = ref<string>("");
const isLoading = ref<boolean>(false);
const key = ref<string>("");
const keyAlreadyUsed = ref<boolean>(false);
const keyField = ref<InstanceType<typeof KeyField> | null>(null);
const name = ref<string>("");
const power = ref<number>();
const powerPoints = ref<number>();
const summary = ref<string>("");
const type = ref<PokemonType | "">("");

const hasChanges = computed<boolean>(() => {
  const move: Move | undefined = props.move;
  return (
    type.value !== (move?.type ?? "") ||
    category.value !== (move?.category ?? "") ||
    key.value !== (move?.key ?? "") ||
    name.value !== (move?.name ?? "") ||
    summary.value !== (move?.summary ?? "") ||
    content.value !== (move?.content ?? "") ||
    (accuracy.value ?? 0) !== (move?.accuracy ?? 0) ||
    (power.value ?? 0) !== (move?.power ?? 0) ||
    (powerPoints.value ?? 0) !== (move?.powerPoints ?? 0)
  );
});

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    try {
      if (props.move) {
        const payload: UpdateMovePayload = {
          key: key.value,
          name: { value: name.value },
          summary: { value: summary.value },
          content: { value: content.value },
          accuracy: { value: accuracy.value || null },
          power: { value: power.value || null },
          powerPoints: { value: powerPoints.value || null },
        };
        const updated: Move = await updateMove(props.move.id, payload);
        emit("updated", updated);
      } else if (type.value && category.value) {
        const payload: CreateOrReplaceMovePayload = {
          type: type.value,
          category: category.value,
          key: key.value,
          name: name.value,
          summary: summary.value,
          content: content.value,
          accuracy: accuracy.value || null,
          power: power.value || null,
          powerPoints: powerPoints.value || null,
        };
        const created: Move = await createMove(payload);
        emit("created", created);
      }
      nextTick(reinitialize);
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
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(name, (newValue, oldValue) => {
  if (!key.value || key.value === slugify(oldValue)) {
    key.value = slugify(newValue);
  }
});
watch(
  () => props.move,
  (move) => {
    type.value = move?.type ?? "";
    category.value = move?.category ?? "";
    key.value = move?.key ?? "";
    name.value = move?.name ?? "";
    summary.value = move?.summary ?? "";
    content.value = move?.content ?? "";
    accuracy.value = move?.accuracy ?? undefined;
    power.value = move?.power ?? undefined;
    powerPoints.value = move?.powerPoints ?? undefined;
  },
  { deep: true, immediate: true },
);
</script>
