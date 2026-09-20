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
    <SummaryField class="mb-3" v-model="summary" />
    <ContentField class="mb-3" v-model="content" />
    <div class="d-flex justify-content-end mb-3">
      <SaveButton
        :disabled="!hasChanges || isLoading"
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

import ContentField from "@/components/shared/ContentField.vue";
import FormCategoryField from "./FormCategoryField.vue";
import KeyAlreadyUsed from "@/components/shared/KeyAlreadyUsed.vue";
import KeyField from "@/components/shared/KeyField.vue";
import NameField from "@/components/shared/NameField.vue";
import SaveButton from "@/components/shared/SaveButton.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TarInput from "@/components/tar/TarInput.vue";
import VarietyField from "@/components/varieties/VarietyField.vue";
import type { AbilitySummary } from "@/types/abilities";
import type { ApiFailure, ProblemDetails } from "@/types/api";
import type { CreateOrReplaceFormPayload, Form, FormCategory, FormFilters, UpdateFormPayload } from "@/types/pokemonForms";
import type { VarietySummary } from "@/types/varieties";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createForm, getFormFilters, updateForm } from "@/api/pokemonForms";
import { formatVariety } from "@/utils/format";
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

const abilities = ref<AbilitySummary[]>([]);
const category = ref<FormCategory | "">("");
const content = ref<string>("");
const isLoading = ref<boolean>(false);
const key = ref<string>("");
const keyAlreadyUsed = ref<boolean>(false);
const keyField = ref<InstanceType<typeof KeyField> | null>(null);
const name = ref<string>("");
const summary = ref<string>("");
const variety = ref<VarietySummary>();
const varietyList = ref<VarietySummary[]>([]);

const varietyOptions = computed<VarietySummary[]>(() => (props.form ? [props.form.variety] : varietyList.value));
const hasChanges = computed<boolean>(() => {
  const form: Form | undefined = props.form;
  return (
    (variety.value?.id ?? "") !== (form?.variety.id ?? "") ||
    category.value !== (form?.category ?? "") ||
    key.value !== (form?.key ?? "") ||
    name.value !== (form?.name ?? "") ||
    summary.value !== (form?.summary ?? "") ||
    content.value !== (form?.content ?? "")
  );
});

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    try {
      if (props.form) {
        const payload: UpdateFormPayload = {
          key: key.value,
          name: { value: name.value },
          summary: { value: summary.value },
          content: { value: content.value },
        };
        const updated: Form = await updateForm(props.form.id, payload);
        emit("updated", updated);
      } else if (variety.value && category.value && abilities.value[0]) {
        const payload: CreateOrReplaceFormPayload = {
          varietyId: variety.value.id,
          category: category.value,
          key: key.value,
          name: name.value,
          summary: summary.value,
          content: content.value,
          types: { primary: "Normal" },
          abilities: { primaryId: abilities.value[0].id },
          baseStatistics: { hp: 1, attack: 1, defense: 1, specialAttack: 1, specialDefense: 1, speed: 1 },
          yield: { experience: 0, hp: 0, attack: 0, defense: 0, specialAttack: 0, specialDefense: 0, speed: 0 },
          size: { height: 0.1, weight: 0.1 },
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
  },
  { deep: true, immediate: true },
);

onMounted(async () => {
  if (!props.form) {
    try {
      const filters: FormFilters = await getFormFilters();
      varietyList.value = [...filters.varieties];
      abilities.value = [...filters.abilities];
    } catch (e: unknown) {
      emit("error", e);
    }
  }
});
</script>
