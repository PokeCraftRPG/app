<template>
  <form @submit.prevent="handleSubmit(submit)">
    <KeyAlreadyUsed v-model="keyAlreadyUsed" />
    <div class="row">
      <div class="col-md-6">
        <TarInput v-if="form" class="mb-3" disabled floating id="variety" :label="t('varieties.label')" :model-value="formatVariety(form.variety)" />
        <VarietyField v-else class="mb-3" :model-value="variety?.id" required :varieties="varietyOptions" @selected="selectVariety" />
      </div>
      <div class="col-md-6">
        <TarInput
          v-if="form"
          class="mb-3"
          disabled
          floating
          id="category"
          :label="t('forms.category.label')"
          :model-value="t(`forms.category.options.${form.category}`)"
        />
        <FormCategoryField v-else class="mb-3" required v-model="category" />
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
        <PokemonTypeField
          class="mb-3"
          id="primary-type"
          label="forms.types.primary"
          :model-value="primaryType"
          required
          @update:model-value="onPrimaryTypeUpdate"
        />
      </div>
      <div class="col-md-6">
        <PokemonTypeField class="mb-3" :exclude="secondaryTypeExclusions" id="secondary-type" label="forms.types.secondary" v-model="secondaryType" />
      </div>
    </div>
    <div class="row">
      <div class="col-md-4">
        <AbilityField
          :abilities="abilityOptions"
          class="mb-3"
          :exclude="primaryAbilityExclusions"
          id="primary-ability"
          label="forms.abilities.primary"
          :model-value="primaryAbility?.id"
          required
          @selected="onPrimaryAbilitySelected"
        />
      </div>
      <div class="col-md-4">
        <AbilityField
          :abilities="abilityOptions"
          class="mb-3"
          :exclude="secondaryAbilityExclusions"
          id="secondary-ability"
          label="forms.abilities.secondary"
          :model-value="secondaryAbility?.id"
          @selected="onSecondaryAbilityUpdate"
        />
      </div>
      <div class="col-md-4">
        <AbilityField
          :abilities="abilityOptions"
          class="mb-3"
          :exclude="hiddenAbilityExclusions"
          id="hidden-ability"
          label="forms.abilities.hidden"
          :model-value="hiddenAbility?.id"
          @selected="hiddenAbility = $event"
        />
      </div>
    </div>
    <div class="row">
      <div class="col-md-6">
        <HeightField class="mb-3" required v-model="height" />
      </div>
      <div class="col-md-6">
        <WeightField class="mb-3" required v-model="weight" />
      </div>
    </div>
    <h2 class="h5">{{ t("forms.statistics.base") }}</h2>
    <div class="row">
      <div class="col-md-4 col-lg-2">
        <BaseStatisticField class="mb-3" statistic="HP" v-model="baseHp" />
      </div>
      <div class="col-md-4 col-lg-2">
        <BaseStatisticField class="mb-3" statistic="Attack" v-model="baseAttack" />
      </div>
      <div class="col-md-4 col-lg-2">
        <BaseStatisticField class="mb-3" statistic="Defense" v-model="baseDefense" />
      </div>
      <div class="col-md-4 col-lg-2">
        <BaseStatisticField class="mb-3" statistic="SpecialAttack" v-model="baseSpecialAttack" />
      </div>
      <div class="col-md-4 col-lg-2">
        <BaseStatisticField class="mb-3" statistic="SpecialDefense" v-model="baseSpecialDefense" />
      </div>
      <div class="col-md-4 col-lg-2">
        <BaseStatisticField class="mb-3" statistic="Speed" v-model="baseSpeed" />
      </div>
    </div>
    <div class="mb-3 text-body-secondary">{{ t("forms.statistics.total", { total: totalBaseStatistics }) }}</div>
    <h2 class="h5">{{ t("forms.statistics.yield.title") }}</h2>
    <div class="row">
      <div class="col-md-4">
        <YieldField class="mb-3" v-model="yieldExperience" />
      </div>
    </div>
    <div class="row">
      <div class="col-md-4 col-lg-2">
        <YieldField class="mb-3" statistic="HP" v-model="yieldHp" />
      </div>
      <div class="col-md-4 col-lg-2">
        <YieldField class="mb-3" statistic="Attack" v-model="yieldAttack" />
      </div>
      <div class="col-md-4 col-lg-2">
        <YieldField class="mb-3" statistic="Defense" v-model="yieldDefense" />
      </div>
      <div class="col-md-4 col-lg-2">
        <YieldField class="mb-3" statistic="SpecialAttack" v-model="yieldSpecialAttack" />
      </div>
      <div class="col-md-4 col-lg-2">
        <YieldField class="mb-3" statistic="SpecialDefense" v-model="yieldSpecialDefense" />
      </div>
      <div class="col-md-4 col-lg-2">
        <YieldField class="mb-3" statistic="Speed" v-model="yieldSpeed" />
      </div>
    </div>
    <div class="mb-3" :class="isTotalYieldValid ? 'text-body-secondary' : 'text-danger'">
      {{ t("forms.statistics.total", { total: totalYield }) }}
      <template v-if="!isTotalYieldValid"> {{ "—" }} {{ t("forms.statistics.yield.invalid") }}</template>
    </div>
    <SummaryField class="mb-3" v-model="summary" />
    <ContentField class="mb-3" v-model="content" />
    <div class="d-flex justify-content-end mb-3">
      <SaveButton
        :disabled="!hasChanges || isLoading || !isTotalYieldValid"
        :icon="form ? 'fas fa-floppy-disk' : 'fas fa-plus'"
        :loading="isLoading"
        :text="form ? 'actions.save' : 'actions.create'"
      />
    </div>
  </form>
</template>

<script setup lang="ts">
import { computed, nextTick, onMounted, ref, watch } from "vue";
import { stringUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import AbilityField from "@/components/abilities/AbilityField.vue";
import BaseStatisticField from "./BaseStatisticField.vue";
import ContentField from "@/components/shared/ContentField.vue";
import FormCategoryField from "./FormCategoryField.vue";
import HeightField from "./HeightField.vue";
import KeyAlreadyUsed from "@/components/shared/KeyAlreadyUsed.vue";
import KeyField from "@/components/shared/KeyField.vue";
import NameField from "@/components/shared/NameField.vue";
import PokemonTypeField from "@/components/pokemon/PokemonTypeField.vue";
import SaveButton from "@/components/shared/SaveButton.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TarInput from "@/components/tar/TarInput.vue";
import VarietyField from "@/components/varieties/VarietyField.vue";
import WeightField from "./WeightField.vue";
import YieldField from "./YieldField.vue";
import type { Ability, AbilitySummary } from "@/types/abilities";
import type { ApiFailure, ProblemDetails } from "@/types/api";
import type {
  BaseStatistics,
  CreateOrReplaceFormPayload,
  Form,
  FormAbilitiesPayload,
  FormCategory,
  FormFilters,
  FormYield,
  UpdateFormPayload,
} from "@/types/pokemonForms";
import type { PokemonType } from "@/types/pokemon";
import type { VarietySummary } from "@/types/varieties";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createForm, getFormFilters, updateForm } from "@/api/pokemonForms";
import { formatVariety } from "@/utils/format";
import { fromTenths, toTenths } from "@/utils/number";
import { useForm } from "@/forms";

const { slugify } = stringUtils;
const { t } = useI18n();

const props = defineProps<{
  form?: Form;
}>();

const emit = defineEmits<{
  (e: "created", value: Form): void;
  (e: "error", value: unknown): void;
  (e: "updated", value: Form): void;
}>();

const abilityList = ref<AbilitySummary[]>([]);
const baseAttack = ref<number>();
const baseDefense = ref<number>();
const baseHp = ref<number>();
const baseSpecialAttack = ref<number>();
const baseSpecialDefense = ref<number>();
const baseSpeed = ref<number>();
const category = ref<FormCategory | "">("");
const content = ref<string>("");
const height = ref<number>();
const hiddenAbility = ref<Ability | AbilitySummary>();
const isLoading = ref<boolean>(false);
const key = ref<string>("");
const keyAlreadyUsed = ref<boolean>(false);
const keyField = ref<InstanceType<typeof KeyField> | null>(null);
const name = ref<string>("");
const primaryAbility = ref<Ability | AbilitySummary>();
const primaryType = ref<PokemonType | "">("");
const secondaryAbility = ref<Ability | AbilitySummary>();
const secondaryType = ref<PokemonType | "">("");
const summary = ref<string>("");
const variety = ref<VarietySummary>();
const varietyList = ref<VarietySummary[]>([]);
const weight = ref<number>();
const yieldAttack = ref<number>();
const yieldDefense = ref<number>();
const yieldExperience = ref<number>();
const yieldHp = ref<number>();
const yieldSpecialAttack = ref<number>();
const yieldSpecialDefense = ref<number>();
const yieldSpeed = ref<number>();

const varietyOptions = computed<VarietySummary[]>(() => (props.form ? [props.form.variety] : varietyList.value));
const abilityOptions = computed<AbilitySummary[]>(() => {
  const byId = new Map(abilityList.value.map((ability) => [ability.id, ability]));
  const form = props.form;
  if (form) {
    byId.set(form.abilities.primary.id, form.abilities.primary);
    if (form.abilities.secondary) {
      byId.set(form.abilities.secondary.id, form.abilities.secondary);
    }
    if (form.abilities.hidden) {
      byId.set(form.abilities.hidden.id, form.abilities.hidden);
    }
  }
  return [...byId.values()];
});
const secondaryTypeExclusions = computed<PokemonType[]>(() => (primaryType.value ? [primaryType.value] : []));
const primaryAbilityExclusions = computed<string[]>(() => [secondaryAbility.value?.id ?? "", hiddenAbility.value?.id ?? ""].filter(Boolean));
const secondaryAbilityExclusions = computed<string[]>(() => [primaryAbility.value?.id ?? "", hiddenAbility.value?.id ?? ""].filter(Boolean));
const hiddenAbilityExclusions = computed<string[]>(() => [primaryAbility.value?.id ?? "", secondaryAbility.value?.id ?? ""].filter(Boolean));
const sizePayload = computed(() => ({
  height: toTenths(height.value) ?? 0,
  weight: toTenths(weight.value) ?? 0,
}));
const baseStatisticsPayload = computed<BaseStatistics>(() => ({
  hp: baseHp.value ?? 0,
  attack: baseAttack.value ?? 0,
  defense: baseDefense.value ?? 0,
  specialAttack: baseSpecialAttack.value ?? 0,
  specialDefense: baseSpecialDefense.value ?? 0,
  speed: baseSpeed.value ?? 0,
}));
const totalBaseStatistics = computed<number>(
  () =>
    (baseHp.value ?? 0) +
    (baseAttack.value ?? 0) +
    (baseDefense.value ?? 0) +
    (baseSpecialAttack.value ?? 0) +
    (baseSpecialDefense.value ?? 0) +
    (baseSpeed.value ?? 0),
);
const yieldPayload = computed<FormYield>(() => ({
  experience: yieldExperience.value ?? 0,
  hp: yieldHp.value ?? 0,
  attack: yieldAttack.value ?? 0,
  defense: yieldDefense.value ?? 0,
  specialAttack: yieldSpecialAttack.value ?? 0,
  specialDefense: yieldSpecialDefense.value ?? 0,
  speed: yieldSpeed.value ?? 0,
}));
const totalYield = computed<number>(
  () =>
    (yieldHp.value ?? 0) +
    (yieldAttack.value ?? 0) +
    (yieldDefense.value ?? 0) +
    (yieldSpecialAttack.value ?? 0) +
    (yieldSpecialDefense.value ?? 0) +
    (yieldSpeed.value ?? 0),
);
const isTotalYieldValid = computed<boolean>(() => totalYield.value >= 1 && totalYield.value <= 4);
const hasChanges = computed<boolean>(() => {
  const form: Form | undefined = props.form;
  return (
    (variety.value?.id ?? "") !== (form?.variety.id ?? "") ||
    category.value !== (form?.category ?? "") ||
    key.value !== (form?.key ?? "") ||
    name.value !== (form?.name ?? "") ||
    summary.value !== (form?.summary ?? "") ||
    content.value !== (form?.content ?? "") ||
    primaryType.value !== (form?.types.primary ?? "") ||
    secondaryType.value !== (form?.types.secondary ?? "") ||
    (primaryAbility.value?.id ?? "") !== (form?.abilities.primary.id ?? "") ||
    (secondaryAbility.value?.id ?? "") !== (form?.abilities.secondary?.id ?? "") ||
    (hiddenAbility.value?.id ?? "") !== (form?.abilities.hidden?.id ?? "") ||
    (height.value ?? 0) !== (fromTenths(form?.size.height) ?? 0) ||
    (weight.value ?? 0) !== (fromTenths(form?.size.weight) ?? 0) ||
    (baseHp.value ?? 0) !== (form?.baseStatistics.hp ?? 0) ||
    (baseAttack.value ?? 0) !== (form?.baseStatistics.attack ?? 0) ||
    (baseDefense.value ?? 0) !== (form?.baseStatistics.defense ?? 0) ||
    (baseSpecialAttack.value ?? 0) !== (form?.baseStatistics.specialAttack ?? 0) ||
    (baseSpecialDefense.value ?? 0) !== (form?.baseStatistics.specialDefense ?? 0) ||
    (baseSpeed.value ?? 0) !== (form?.baseStatistics.speed ?? 0) ||
    (yieldExperience.value ?? 0) !== (form?.yield.experience ?? 0) ||
    (yieldHp.value ?? 0) !== (form?.yield.hp ?? 0) ||
    (yieldAttack.value ?? 0) !== (form?.yield.attack ?? 0) ||
    (yieldDefense.value ?? 0) !== (form?.yield.defense ?? 0) ||
    (yieldSpecialAttack.value ?? 0) !== (form?.yield.specialAttack ?? 0) ||
    (yieldSpecialDefense.value ?? 0) !== (form?.yield.specialDefense ?? 0) ||
    (yieldSpeed.value ?? 0) !== (form?.yield.speed ?? 0)
  );
});

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && primaryType.value && primaryAbility.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    try {
      const abilitiesPayload: FormAbilitiesPayload = {
        primaryId: primaryAbility.value?.id,
        secondaryId: secondaryAbility.value?.id,
        hiddenId: hiddenAbility.value?.id,
      };
      if (props.form) {
        const payload: UpdateFormPayload = {
          key: key.value,
          name: { value: name.value },
          summary: { value: summary.value },
          content: { value: content.value },
          types: {
            primary: primaryType.value,
            secondary: secondaryType.value || null,
          },
          abilities: abilitiesPayload,
          baseStatistics: baseStatisticsPayload.value,
          yield: yieldPayload.value,
          size: sizePayload.value,
        };
        const updated: Form = await updateForm(props.form.id, payload);
        emit("updated", updated);
      } else if (variety.value && category.value) {
        const payload: CreateOrReplaceFormPayload = {
          varietyId: variety.value.id,
          category: category.value,
          key: key.value,
          name: name.value,
          summary: summary.value,
          content: content.value,
          types: {
            primary: primaryType.value,
            secondary: secondaryType.value || null,
          },
          abilities: abilitiesPayload,
          baseStatistics: baseStatisticsPayload.value,
          yield: yieldPayload.value,
          size: sizePayload.value,
        };
        const created: Form = await createForm(payload);
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

function onPrimaryTypeUpdate(value: PokemonType | ""): void {
  primaryType.value = value;
  if (value && value === secondaryType.value) {
    secondaryType.value = "";
  }
}

function onPrimaryAbilitySelected(ability: AbilitySummary | undefined): void {
  primaryAbility.value = ability;
  if (ability && ability.id === secondaryAbility.value?.id) {
    secondaryAbility.value = undefined;
  }
  if (ability && ability.id === hiddenAbility.value?.id) {
    hiddenAbility.value = undefined;
  }
}

function onSecondaryAbilityUpdate(ability: AbilitySummary | undefined): void {
  secondaryAbility.value = ability;
  if (ability && ability.id === hiddenAbility.value?.id) {
    hiddenAbility.value = undefined;
  }
}

function selectVariety(value: VarietySummary | undefined): void {
  const previousName: string = variety.value ? formatVariety(variety.value) : "";
  variety.value = value;
  if (value && (!name.value || name.value === previousName)) {
    name.value = formatVariety(value);
  }
}

watch(name, (newValue, oldValue) => {
  if (!key.value || key.value === slugify(oldValue)) {
    key.value = slugify(newValue);
  }
});
watch(
  () => props.form,
  (form) => {
    variety.value = form?.variety;
    category.value = form?.category ?? "";
    key.value = form?.key ?? "";
    name.value = form?.name ?? "";
    summary.value = form?.summary ?? "";
    content.value = form?.content ?? "";
    primaryType.value = form?.types.primary ?? "";
    secondaryType.value = form?.types.secondary ?? "";
    primaryAbility.value = form?.abilities.primary;
    secondaryAbility.value = form?.abilities.secondary ?? undefined;
    hiddenAbility.value = form?.abilities.hidden ?? undefined;
    height.value = fromTenths(form?.size.height);
    weight.value = fromTenths(form?.size.weight);
    baseHp.value = form?.baseStatistics.hp;
    baseAttack.value = form?.baseStatistics.attack;
    baseDefense.value = form?.baseStatistics.defense;
    baseSpecialAttack.value = form?.baseStatistics.specialAttack;
    baseSpecialDefense.value = form?.baseStatistics.specialDefense;
    baseSpeed.value = form?.baseStatistics.speed;
    yieldExperience.value = form?.yield.experience;
    yieldHp.value = form?.yield.hp;
    yieldAttack.value = form?.yield.attack;
    yieldDefense.value = form?.yield.defense;
    yieldSpecialAttack.value = form?.yield.specialAttack;
    yieldSpecialDefense.value = form?.yield.specialDefense;
    yieldSpeed.value = form?.yield.speed;
  },
  { deep: true, immediate: true },
);

onMounted(async () => {
  try {
    const filters: FormFilters = await getFormFilters();
    varietyList.value = [...filters.varieties];
    abilityList.value = [...filters.abilities];
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>
