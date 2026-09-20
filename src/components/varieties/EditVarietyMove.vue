<template>
  <div>
    <TarModal centered :close="t('actions.close')" fade scrollable ref="modal" :title="title">
      <KeyAlreadyUsed v-model="alreadyUsed" help="varieties.moves.alreadyUsed.help" lead="varieties.moves.alreadyUsed.lead" />
      <form @submit.prevent="handleSubmit(submit)">
        <TarInput v-if="varietyMove" class="mb-3" disabled floating id="move" :label="t('moves.label')" :model-value="formatMove(varietyMove.move)" />
        <MoveField v-else class="mb-3" :model-value="move?.id" :moves="moves" required @selected="move = $event" />
        <LearningMethodField class="mb-3" required v-model="learningMethod" />
        <LevelField v-if="learningMethod === 'LevelUp'" class="mb-3" required v-model="level" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="!canSubmit || isLoading"
          :icon="varietyMove ? 'fas fa-floppy-disk' : 'fas fa-plus'"
          :loading="isLoading"
          :status="t('loading')"
          :text="varietyMove ? t('actions.save') : t('actions.add')"
          @click="handleSubmit(submit)"
        />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { computed, nextTick, ref } from "vue";
import { useI18n } from "vue-i18n";

import KeyAlreadyUsed from "@/components/shared/KeyAlreadyUsed.vue";
import LearningMethodField from "./LearningMethodField.vue";
import LevelField from "./LevelField.vue";
import MoveField from "@/components/moves/MoveField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarInput from "@/components/tar/TarInput.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { ApiFailure, ProblemDetails } from "@/types/api";
import type { MoveSummary } from "@/types/moves";
import type { LearningMethod, SetVarietyMovePayload, Variety, VarietyMove } from "@/types/varieties";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { addVarietyMove, setVarietyMove } from "@/api/varieties";
import { formatMove } from "@/utils/format";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  moves?: MoveSummary[];
  varietyId: string;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "saved", value: Variety): void;
}>();

const alreadyUsed = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const learningMethod = ref<LearningMethod | "">("");
const level = ref<number>(0);
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const move = ref<MoveSummary>();
const varietyMove = ref<VarietyMove>();

const canSubmit = computed<boolean>(
  () => !varietyMove.value || learningMethod.value !== varietyMove.value.learningMethod || level.value !== (varietyMove.value.level ?? 0),
);
const title = computed<string>(() => (varietyMove.value ? t("varieties.moves.edit", { move: formatMove(varietyMove.value.move) }) : t("varieties.moves.add")));

function cancel(): void {
  clear();
  modal.value?.hide();
}

function clear(): void {
  reset();
  alreadyUsed.value = false;
  move.value = undefined;
  varietyMove.value = undefined;
}

function open(entry?: VarietyMove): void {
  varietyMove.value = entry;
  move.value = entry?.move;
  learningMethod.value = entry?.learningMethod ?? "";
  level.value = entry?.level ?? 0;
  alreadyUsed.value = false;
  nextTick(() => {
    reinitialize();
    modal.value?.show();
  });
}
defineExpose({ open });

const { handleSubmit, reinitialize, reset } = useForm();
async function submit(): Promise<void> {
  const moveId: string | undefined = varietyMove.value?.move.id ?? move.value?.id;
  if (!isLoading.value && canSubmit.value && learningMethod.value && moveId) {
    isLoading.value = true;
    alreadyUsed.value = false;
    try {
      const payload: SetVarietyMovePayload = {
        moveId,
        learningMethod: learningMethod.value,
        level: learningMethod.value === "LevelUp" ? level.value : null,
      };
      const variety: Variety = varietyMove.value
        ? await setVarietyMove(props.varietyId, varietyMove.value.id, payload)
        : await addVarietyMove(props.varietyId, payload);
      clear();
      modal.value?.hide();
      emit("saved", variety);
    } catch (e: unknown) {
      const failure = e as ApiFailure;
      if (failure.status === StatusCodes.Conflict) {
        const problemDetails = failure.data as ProblemDetails;
        if (problemDetails.error?.code === ErrorCodes.DuplicateVarietyMove) {
          alreadyUsed.value = true;
          return;
        }
      }
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>
