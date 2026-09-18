<template>
  <form @submit.prevent="handleSubmit(submit)">
    <KeyAlreadyUsed v-model="keyAlreadyUsed" />
    <KeyAlreadyUsed v-model="numberAlreadyUsed" help="species.number.alreadyUsed.help" lead="species.number.alreadyUsed.lead" />
    <div class="row">
      <div class="col-md-6">
        <NumberField class="mb-3" :disabled="Boolean(species)" ref="numberField" required v-model="number" />
      </div>
      <div class="col-md-6">
        <SpeciesCategoryField class="mb-3" :disabled="Boolean(species)" required v-model="category" />
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
        <BaseFriendshipField class="mb-3" required v-model="baseFriendship" />
      </div>
      <div class="col-md-4">
        <CatchRateField class="mb-3" required v-model="catchRate" />
      </div>
      <div class="col-md-4">
        <GrowthRateField class="mb-3" required v-model="growthRate" />
      </div>
    </div>
    <div class="row">
      <div class="col-md-4">
        <EggCyclesField class="mb-3" required v-model="eggCycles" />
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
      <SaveButton
        :disabled="!hasChanges || isLoading"
        :icon="species ? 'fas fa-floppy-disk' : 'fas fa-plus'"
        :loading="isLoading"
        :text="species ? 'actions.save' : 'actions.create'"
      />
    </div>
  </form>
</template>

<script setup lang="ts">
import { computed, nextTick, ref, watch } from "vue";
import { stringUtils } from "logitar-js";

import BaseFriendshipField from "./BaseFriendshipField.vue";
import CatchRateField from "./CatchRateField.vue";
import ContentField from "@/components/shared/ContentField.vue";
import EggCyclesField from "./EggCyclesField.vue";
import EggGroupField from "./EggGroupField.vue";
import GrowthRateField from "./GrowthRateField.vue";
import KeyAlreadyUsed from "@/components/shared/KeyAlreadyUsed.vue";
import KeyField from "@/components/shared/KeyField.vue";
import NameField from "@/components/shared/NameField.vue";
import NumberField from "./NumberField.vue";
import SaveButton from "@/components/shared/SaveButton.vue";
import SpeciesCategoryField from "./SpeciesCategoryField.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import type { ApiFailure, ProblemDetails } from "@/types/api";
import type { CreateOrReplaceSpeciesPayload, EggGroup, GrowthRate, Species, SpeciesCategory, UpdateSpeciesPayload } from "@/types/species";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createSpecies, updateSpecies } from "@/api/species";
import { useForm } from "@/forms";

const { slugify } = stringUtils;

const props = defineProps<{
  species?: Species;
}>();

const emit = defineEmits<{
  (e: "created", value: Species): void;
  (e: "error", value: unknown): void;
  (e: "updated", value: Species): void;
}>();

const baseFriendship = ref<number>(0);
const catchRate = ref<number>(0);
const category = ref<SpeciesCategory | "">("");
const content = ref<string>("");
const eggCycles = ref<number>(0);
const growthRate = ref<GrowthRate | "">("");
const isLoading = ref<boolean>(false);
const key = ref<string>("");
const keyAlreadyUsed = ref<boolean>(false);
const keyField = ref<InstanceType<typeof KeyField> | null>(null);
const name = ref<string>("");
const number = ref<number>(0);
const numberAlreadyUsed = ref<boolean>(false);
const numberField = ref<InstanceType<typeof NumberField> | null>(null);
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
const hasChanges = computed<boolean>(() => {
  const species: Species | undefined = props.species;
  return (
    number.value !== (species?.number ?? 0) ||
    category.value !== (species?.category ?? "") ||
    key.value !== (species?.key ?? "") ||
    name.value !== (species?.name ?? "") ||
    summary.value !== (species?.summary ?? "") ||
    content.value !== (species?.content ?? "") ||
    baseFriendship.value !== (species?.baseFriendship ?? 0) ||
    catchRate.value !== (species?.catchRate ?? 0) ||
    growthRate.value !== (species?.growthRate ?? "") ||
    eggCycles.value !== (species?.eggs.cycles ?? 0) ||
    primaryEggGroup.value !== (species?.eggs.primaryGroup ?? "") ||
    secondaryEggGroup.value !== (species?.eggs.secondaryGroup ?? "")
  );
});

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && growthRate.value && primaryEggGroup.value && category.value && number.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    numberAlreadyUsed.value = false;
    try {
      if (props.species) {
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
        const updated: Species = await updateSpecies(props.species.id, payload);
        emit("updated", updated);
      } else {
        const payload: CreateOrReplaceSpeciesPayload = {
          number: number.value,
          category: category.value,
          key: key.value,
          name: name.value,
          summary: summary.value,
          content: content.value,
          baseFriendship: baseFriendship.value,
          catchRate: catchRate.value,
          growthRate: growthRate.value,
          eggs: {
            cycles: eggCycles.value,
            primaryGroup: primaryEggGroup.value,
            secondaryGroup: isSecondaryEggDisabled.value ? null : secondaryEggGroup.value || null,
          },
        };
        const created: Species = await createSpecies(payload);
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

watch(name, (newValue, oldValue) => {
  if (!key.value || key.value === slugify(oldValue)) {
    key.value = slugify(newValue);
  }
});
watch(primaryEggGroup, (value) => {
  if (value === "NoEggsDiscovered" || value === "Ditto" || value === secondaryEggGroup.value) {
    secondaryEggGroup.value = "";
  }
});
watch(
  () => props.species,
  (species) => {
    number.value = species?.number ?? 0;
    category.value = species?.category ?? "";
    key.value = species?.key ?? "";
    name.value = species?.name ?? "";
    summary.value = species?.summary ?? "";
    content.value = species?.content ?? "";
    baseFriendship.value = species?.baseFriendship ?? 0;
    catchRate.value = species?.catchRate ?? 0;
    growthRate.value = species?.growthRate ?? "";
    eggCycles.value = species?.eggs.cycles ?? 0;
    primaryEggGroup.value = species?.eggs.primaryGroup ?? "";
    secondaryEggGroup.value = species?.eggs.secondaryGroup ?? "";
  },
  { deep: true, immediate: true },
);
</script>
