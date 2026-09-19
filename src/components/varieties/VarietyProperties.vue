<template>
  <form @submit.prevent="handleSubmit(submit)">
    <KeyAlreadyUsed v-model="keyAlreadyUsed" />
    <SpeciesField class="mb-3" :disabled="Boolean(variety)" :model-value="species?.id" :species="speciesOptions" required @selected="selectSpecies" />
    <div class="row">
      <div class="col-md-6">
        <TarCheckbox class="mb-3" :label="t('varieties.default.label')" switch v-model="isDefault" />
      </div>
      <div class="col-md-6">
        <TarCheckbox class="mb-3" :label="t('varieties.canChangeForm')" switch v-model="canChangeForm" />
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
      <div class="col-md-6">
        <GenusField class="mb-3" v-model="genus" />
      </div>
      <div class="col-md-6">
        <GenderRatioField class="mb-3" v-model="genderRatio" />
      </div>
    </div>
    <SummaryField class="mb-3" v-model="summary" />
    <ContentField class="mb-3" v-model="content" />
    <div class="d-flex justify-content-end mb-3">
      <SaveButton
        :disabled="!hasChanges || isLoading"
        :icon="variety ? 'fas fa-floppy-disk' : 'fas fa-plus'"
        :loading="isLoading"
        :text="variety ? 'actions.save' : 'actions.create'"
      />
    </div>
  </form>
</template>

<script setup lang="ts">
import { computed, nextTick, onMounted, ref, watch } from "vue";
import { stringUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import ContentField from "@/components/shared/ContentField.vue";
import GenderRatioField from "./GenderRatioField.vue";
import GenusField from "./GenusField.vue";
import KeyAlreadyUsed from "@/components/shared/KeyAlreadyUsed.vue";
import KeyField from "@/components/shared/KeyField.vue";
import NameField from "@/components/shared/NameField.vue";
import SaveButton from "@/components/shared/SaveButton.vue";
import SpeciesField from "@/components/species/SpeciesField.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TarCheckbox from "@/components/tar/TarCheckbox.vue";
import type { ApiFailure, ProblemDetails } from "@/types/api";
import type { SpeciesSummary } from "@/types/species";
import type { CreateOrReplaceVarietyPayload, UpdateVarietyPayload, Variety } from "@/types/varieties";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createVariety, getVarietyFilters, updateVariety } from "@/api/varieties";
import { formatSpecies } from "@/utils/format";
import { useForm } from "@/forms";

const { slugify } = stringUtils;
const { n, t } = useI18n();

const props = defineProps<{
  variety?: Variety;
}>();

const emit = defineEmits<{
  (e: "created", value: Variety): void;
  (e: "error", value: unknown): void;
  (e: "updated", value: Variety): void;
}>();

const canChangeForm = ref<boolean>(false);
const content = ref<string>("");
const genderRatio = ref<number | null>(null);
const genus = ref<string>("");
const isDefault = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const key = ref<string>("");
const keyAlreadyUsed = ref<boolean>(false);
const keyField = ref<InstanceType<typeof KeyField> | null>(null);
const name = ref<string>("");
const species = ref<SpeciesSummary>();
const speciesList = ref<SpeciesSummary[]>([]);
const summary = ref<string>("");

const speciesOptions = computed<SpeciesSummary[]>(() => (props.variety ? [props.variety.species] : speciesList.value));
const hasChanges = computed<boolean>(() => {
  const variety: Variety | undefined = props.variety;
  return (
    (species.value?.id ?? "") !== (variety?.species.id ?? "") ||
    key.value !== (variety?.key ?? "") ||
    name.value !== (variety?.name ?? "") ||
    summary.value !== (variety?.summary ?? "") ||
    content.value !== (variety?.content ?? "") ||
    isDefault.value !== (variety?.isDefault ?? false) ||
    canChangeForm.value !== (variety?.canChangeForm ?? false) ||
    genderRatio.value !== (variety?.genderRatio ?? null) ||
    genus.value !== (variety?.genus ?? "")
  );
});

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && species.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    try {
      if (props.variety) {
        const payload: UpdateVarietyPayload = {
          isDefault: isDefault.value,
          key: key.value,
          name: { value: name.value },
          summary: { value: summary.value },
          content: { value: content.value },
          canChangeForm: canChangeForm.value,
          genderRatio: { value: genderRatio.value },
          genus: { value: genus.value },
        };
        const updated: Variety = await updateVariety(props.variety.id, payload);
        emit("updated", updated);
      } else {
        const payload: CreateOrReplaceVarietyPayload = {
          speciesId: species.value.id,
          isDefault: isDefault.value,
          key: key.value,
          name: name.value,
          summary: summary.value,
          content: content.value,
          canChangeForm: canChangeForm.value,
          genderRatio: genderRatio.value,
          genus: genus.value,
        };
        const created: Variety = await createVariety(payload);
        emit("created", created);
      }
      nextTick(reinitialize);
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

function selectSpecies(value: SpeciesSummary | undefined): void {
  const previousName: string = species.value ? formatSpecies(species.value, n) : "";
  species.value = value;
  if (value && (!name.value || name.value === previousName)) {
    name.value = formatSpecies(value, n);
  }
}

watch(name, (newValue, oldValue) => {
  if (!key.value || key.value === slugify(oldValue)) {
    key.value = slugify(newValue);
  }
});
watch(
  () => props.variety,
  (variety) => {
    species.value = variety?.species;
    key.value = variety?.key ?? "";
    name.value = variety?.name ?? "";
    summary.value = variety?.summary ?? "";
    content.value = variety?.content ?? "";
    isDefault.value = variety?.isDefault ?? false;
    canChangeForm.value = variety?.canChangeForm ?? false;
    genderRatio.value = variety?.genderRatio ?? null;
    genus.value = variety?.genus ?? "";
  },
  { deep: true, immediate: true },
);

onMounted(async () => {
  if (!props.variety) {
    try {
      const filters = await getVarietyFilters();
      speciesList.value = [...filters.species];
    } catch (e: unknown) {
      emit("error", e);
    }
  }
});
</script>
