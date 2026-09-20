<template>
  <div>
    <TarModal centered :close="t('actions.close')" fade scrollable ref="modal" :title="title">
      <KeyAlreadyUsed v-model="numberAlreadyUsed" help="species.number.alreadyUsed.help" lead="species.number.alreadyUsed.lead" />
      <form @submit.prevent="handleSubmit(submit)">
        <TarInput
          v-if="regionalNumber"
          class="mb-3"
          disabled
          floating
          id="region"
          :label="t('regions.label')"
          :model-value="formatRegion(regionalNumber.region)"
        />
        <RegionField v-else class="mb-3" :model-value="region?.id" :regions="regions" required @selected="region = $event" />
        <NumberField class="mb-3" ref="numberField" required v-model="number" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="!canSubmit || isLoading"
          :icon="regionalNumber ? 'fas fa-floppy-disk' : 'fas fa-plus'"
          :loading="isLoading"
          :status="t('loading')"
          :text="regionalNumber ? t('actions.save') : t('actions.add')"
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
import NumberField from "./NumberField.vue";
import RegionField from "@/components/regions/RegionField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarInput from "@/components/tar/TarInput.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { ApiFailure, ProblemDetails } from "@/types/api";
import type { RegionSummary } from "@/types/regions";
import type { RegionalNumber, SetRegionalNumberPayload, Species } from "@/types/species";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { formatPokemonNumber, formatRegion } from "@/utils/format";
import { setRegionalNumber } from "@/api/species";
import { useForm } from "@/forms";

const { n, t } = useI18n();

const props = defineProps<{
  regions?: RegionSummary[];
  speciesId: string;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "saved", value: Species): void;
}>();

const isLoading = ref<boolean>(false);
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const number = ref<number>(0);
const numberAlreadyUsed = ref<boolean>(false);
const numberField = ref<InstanceType<typeof NumberField> | null>(null);
const regionalNumber = ref<RegionalNumber>();
const region = ref<RegionSummary>();

const canSubmit = computed<boolean>(() => !regionalNumber.value || number.value !== regionalNumber.value.number);
const title = computed<string>(() =>
  regionalNumber.value
    ? t("species.regionalNumbers.edit", {
        number: formatPokemonNumber(regionalNumber.value.number, n),
        region: formatRegion(regionalNumber.value.region),
      })
    : t("species.regionalNumbers.add"),
);

function cancel(): void {
  clear();
  modal.value?.hide();
}

function clear(): void {
  reset();
  numberAlreadyUsed.value = false;
  regionalNumber.value = undefined;
  region.value = undefined;
}

function open(entry?: RegionalNumber): void {
  regionalNumber.value = entry;
  region.value = entry?.region;
  console.log(region.value);
  number.value = entry?.number ?? 0;
  numberAlreadyUsed.value = false;
  nextTick(() => {
    reinitialize();
    modal.value?.show();
  });
}
defineExpose({ open });

const { handleSubmit, reinitialize, reset } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && canSubmit.value && region.value) {
    isLoading.value = true;
    numberAlreadyUsed.value = false;
    try {
      const payload: SetRegionalNumberPayload = { number: number.value };
      const species: Species = await setRegionalNumber(props.speciesId, region.value.id, payload);
      clear();
      modal.value?.hide();
      emit("saved", species);
    } catch (e: unknown) {
      const failure = e as ApiFailure;
      if (failure.status === StatusCodes.Conflict) {
        const problemDetails = failure.data as ProblemDetails;
        if (problemDetails.error?.code === ErrorCodes.NumberAlreadyUsed) {
          numberAlreadyUsed.value = true;
          numberField.value?.focus();
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
