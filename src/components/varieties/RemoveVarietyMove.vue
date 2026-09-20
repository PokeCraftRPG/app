<template>
  <div>
    <TarModal centered :close="t('actions.close')" fade scrollable ref="modal" :title="t('varieties.moves.remove.lead')">
      <p class="mb-0">{{ help }}</p>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="isLoading"
          icon="fas fa-xmark"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('actions.remove')"
          variant="danger"
          @click="remove"
        />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { computed, ref } from "vue";
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { Variety, VarietyMove } from "@/types/varieties";
import { formatMove } from "@/utils/format";
import { removeVarietyMove } from "@/api/varieties";

const { t } = useI18n();

const props = defineProps<{
  varietyId: string;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "removed", value: Variety): void;
}>();

const isLoading = ref<boolean>(false);
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const varietyMove = ref<VarietyMove>();

const help = computed<string>(() =>
  t("varieties.moves.remove.help", {
    move: varietyMove.value ? formatMove(varietyMove.value.move) : "",
  }),
);

function cancel(): void {
  varietyMove.value = undefined;
  modal.value?.hide();
}

function open(entry: VarietyMove): void {
  varietyMove.value = entry;
  modal.value?.show();
}
defineExpose({ open });

async function remove(): Promise<void> {
  if (!isLoading.value && varietyMove.value) {
    isLoading.value = true;
    try {
      const variety: Variety = await removeVarietyMove(props.varietyId, varietyMove.value.id);
      varietyMove.value = undefined;
      modal.value?.hide();
      emit("removed", variety);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>
