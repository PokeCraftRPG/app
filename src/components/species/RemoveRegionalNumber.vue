<template>
  <div>
    <TarModal centered :close="t('actions.close')" fade scrollable ref="modal" :title="t('species.regionalNumbers.remove.lead')">
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
import type { RegionalNumber, Species } from "@/types/species";
import { formatPokemonNumber, formatRegion } from "@/utils/format";
import { removeRegionalNumber } from "@/api/species";

const { n, t } = useI18n();

const props = defineProps<{
  speciesId: string;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "removed", value: Species): void;
}>();

const isLoading = ref<boolean>(false);
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const regionalNumber = ref<RegionalNumber>();

const help = computed<string>(() =>
  t("species.regionalNumbers.remove.help", {
    number: formatPokemonNumber(regionalNumber.value?.number ?? 0, n),
    region: regionalNumber.value ? formatRegion(regionalNumber.value.region) : "",
  }),
);

function cancel(): void {
  regionalNumber.value = undefined;
  modal.value?.hide();
}

function open(entry: RegionalNumber): void {
  regionalNumber.value = entry;
  modal.value?.show();
}
defineExpose({ open });

async function remove(): Promise<void> {
  if (!isLoading.value && regionalNumber.value) {
    isLoading.value = true;
    try {
      const species: Species = await removeRegionalNumber(props.speciesId, regionalNumber.value.region.id);
      regionalNumber.value = undefined;
      modal.value?.hide();
      emit("removed", species);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}
</script>
