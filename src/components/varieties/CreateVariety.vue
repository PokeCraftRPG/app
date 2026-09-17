<template>
  <div>
    <TarButton icon="fas fa-plus" size="large" :text="t('actions.create')" @click="open" />
    <TarModal centered :close="t('actions.close')" fade scrollable ref="modal" :title="t('varieties.create')">
      <KeyAlreadyUsed v-model="keyAlreadyUsed" />
      <form @submit.prevent="handleSubmit(submit)">
        <SpeciesField class="mb-3" :model-value="species?.id" :species="speciesOptions" required @selected="selectSpecies" />
        <NameField class="mb-3" :model-value="name" required @update:model-value="updateName" />
        <KeyField class="mb-3" ref="keyField" required v-model="key" />
        <TarCheckbox class="mb-3" :label="t('varieties.default.is')" switch v-model="isDefault" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="cancel" />
        <TarButton
          :disabled="isLoading"
          icon="fas fa-plus"
          :loading="isLoading"
          :status="t('loading')"
          :text="t('actions.create')"
          @click="handleSubmit(submit)"
        />
      </template>
    </TarModal>
  </div>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue";
import { stringUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import KeyAlreadyUsed from "@/components/shared/KeyAlreadyUsed.vue";
import KeyField from "@/components/shared/KeyField.vue";
import NameField from "@/components/shared/NameField.vue";
import SpeciesField from "@/components/species/SpeciesField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarCheckbox from "@/components/tar/TarCheckbox.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { ApiFailure, ProblemDetails } from "@/types/api";
import type { SpeciesSummary } from "@/types/species";
import type { CreateOrReplaceVarietyPayload, Variety } from "@/types/varieties";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createVariety, getVarietyFilters } from "@/api/varieties";
import { useForm } from "@/forms";
import { formatSpecies } from "@/utils/format";

const { slugify } = stringUtils;
const { n, t } = useI18n();

const emit = defineEmits<{
  (e: "created", value: Variety): void;
  (e: "error", value: unknown): void;
}>();

const isDefault = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const key = ref<string>("");
const keyAlreadyUsed = ref<boolean>(false);
const keyField = ref<InstanceType<typeof KeyField> | null>(null);
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");
const species = ref<SpeciesSummary>();
const speciesOptions = ref<SpeciesSummary[]>([]);

function clear(): void {
  reset();
  keyAlreadyUsed.value = false;
  isDefault.value = false;
  key.value = "";
  name.value = "";
  species.value = undefined;
}

function cancel(): void {
  clear();
  modal.value?.hide();
}

function selectSpecies(value: SpeciesSummary | undefined): void {
  const previousName: string = species.value ? formatSpecies(species.value, n) : "";
  species.value = value;
  if (value && (!name.value || name.value === previousName)) {
    updateName(formatSpecies(value, n));
  }
}

function open(): void {
  modal.value?.show();
}

function updateName(value: string): void {
  name.value = value;
  key.value = slugify(value);
}

const { handleSubmit, reset } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && species.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    try {
      const payload: CreateOrReplaceVarietyPayload = {
        speciesId: species.value.id,
        isDefault: isDefault.value,
        key: key.value,
        name: name.value,
        canChangeForm: false,
      };
      const variety: Variety = await createVariety(payload);
      clear();
      modal.value?.hide();
      emit("created", variety);
    } catch (e: unknown) {
      const failure = e as ApiFailure;
      if (failure.status === StatusCodes.Conflict) {
        const problemDetails = failure.data as ProblemDetails;
        if (problemDetails.error?.code === ErrorCodes.KeyAlreadyUsed) {
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

onMounted(async () => {
  try {
    const filters = await getVarietyFilters();
    speciesOptions.value = [...filters.species];
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>
