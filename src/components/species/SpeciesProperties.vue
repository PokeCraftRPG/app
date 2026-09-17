<template>
  <form @submit.prevent="handleSubmit(submit)">
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
        <BaseFriendshipField class="mb-3" v-model="baseFriendship" />
      </div>
      <div class="col-md-4">
        <CatchRateField class="mb-3" v-model="catchRate" />
      </div>
      <div class="col-md-4">
        <GrowthRateField class="mb-3" required v-model="growthRate" />
      </div>
    </div>
    <div class="row">
      <div class="col-md-4">
        <EggCyclesField class="mb-3" v-model="eggCycles" />
      </div>
      <div class="col-md-4">
        <EggGroupField class="mb-3" id="primary-egg-group" label="species.egg.primary" required v-model="primaryEggGroup" />
      </div>
      <div class="col-md-4">
        <EggGroupField
          class="mb-3"
          :disabled="isSecondaryEggDisabled"
          :exclude="secondaryEggExclusions"
          id="secondary-egg-group"
          label="species.egg.secondary"
          v-model="secondaryEggGroup"
        />
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
</template>

<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useI18n } from "vue-i18n";

import BaseFriendshipField from "@/components/species/BaseFriendshipField.vue";
import CatchRateField from "@/components/species/CatchRateField.vue";
import ContentField from "@/components/shared/ContentField.vue";
import EggCyclesField from "@/components/species/EggCyclesField.vue";
import EggGroupField from "@/components/species/EggGroupField.vue";
import GrowthRateField from "@/components/species/GrowthRateField.vue";
import KeyAlreadyUsed from "@/components/shared/KeyAlreadyUsed.vue";
import KeyField from "@/components/shared/KeyField.vue";
import NameField from "@/components/shared/NameField.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { ApiFailure, ProblemDetails } from "@/types/api";
import type { EggGroup, GrowthRate, Species, UpdateSpeciesPayload } from "@/types/species";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { updateSpecies } from "@/api/species";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  species: Species;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Species): void;
}>();

const baseFriendship = ref<number>(0);
const catchRate = ref<number>(1);
const content = ref<string>("");
const eggCycles = ref<number>(1);
const growthRate = ref<GrowthRate | "">("");
const isLoading = ref<boolean>(false);
const key = ref<string>("");
const keyAlreadyUsed = ref<boolean>(false);
const keyField = ref<InstanceType<typeof KeyField> | null>(null);
const name = ref<string>("");
const primaryEggGroup = ref<EggGroup | "">("");
const secondaryEggGroup = ref<EggGroup | "">("");
const summary = ref<string>("");

const isSecondaryEggDisabled = computed<boolean>(() => primaryEggGroup.value === "NoEggsDiscovered" || primaryEggGroup.value === "Ditto");
const secondaryEggExclusions = computed<EggGroup[]>(() => {
  const exclusions: EggGroup[] = ["NoEggsDiscovered", "Ditto"];
  if (primaryEggGroup.value) {
    exclusions.push(primaryEggGroup.value);
  }
  return exclusions;
});
const hasChanges = computed<boolean>(() =>
  Boolean(
    growthRate.value &&
    primaryEggGroup.value &&
    (props.species.key !== key.value ||
      (props.species.name ?? "") !== name.value ||
      (props.species.summary ?? "") !== summary.value ||
      (props.species.content ?? "") !== content.value ||
      props.species.baseFriendship !== baseFriendship.value ||
      props.species.catchRate !== catchRate.value ||
      props.species.growthRate !== growthRate.value ||
      props.species.eggs.cycles !== eggCycles.value ||
      props.species.eggs.primaryGroup !== primaryEggGroup.value ||
      (props.species.eggs.secondaryGroup ?? "") !== secondaryEggGroup.value),
  ),
);

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && growthRate.value && primaryEggGroup.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    try {
      const payload: UpdateSpeciesPayload = {
        key: key.value,
        name: { value: name.value },
        summary: { value: summary.value },
        content: { value: content.value },
        baseFriendship: baseFriendship.value,
        catchRate: catchRate.value,
        growthRate: growthRate.value,
        eggs: {
          cycles: eggCycles.value,
          primaryGroup: primaryEggGroup.value,
          secondaryGroup: isSecondaryEggDisabled.value ? null : secondaryEggGroup.value || null,
        },
      };
      const species: Species = await updateSpecies(props.species.id, payload);
      reinitialize();
      emit("updated", species);
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

watch(primaryEggGroup, (value) => {
  if (value === "NoEggsDiscovered" || value === "Ditto" || value === secondaryEggGroup.value) {
    secondaryEggGroup.value = "";
  }
});

watch(
  () => props.species,
  (species) => {
    key.value = species.key;
    name.value = species.name ?? "";
    summary.value = species.summary ?? "";
    content.value = species.content ?? "";
    baseFriendship.value = species.baseFriendship;
    catchRate.value = species.catchRate;
    growthRate.value = species.growthRate;
    eggCycles.value = species.eggs.cycles;
    primaryEggGroup.value = species.eggs.primaryGroup;
    secondaryEggGroup.value = species.eggs.secondaryGroup ?? "";
  },
  { deep: true, immediate: true },
);
</script>
