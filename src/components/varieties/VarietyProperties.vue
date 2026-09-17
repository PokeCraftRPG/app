<template>
  <form @submit.prevent="handleSubmit(submit)">
    <KeyAlreadyUsed v-model="keyAlreadyUsed" />
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

import ContentField from "@/components/shared/ContentField.vue";
import GenderRatioField from "./GenderRatioField.vue";
import GenusField from "./GenusField.vue";
import KeyAlreadyUsed from "@/components/shared/KeyAlreadyUsed.vue";
import KeyField from "@/components/shared/KeyField.vue";
import NameField from "@/components/shared/NameField.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarCheckbox from "@/components/tar/TarCheckbox.vue";
import type { ApiFailure, ProblemDetails } from "@/types/api";
import type { UpdateVarietyPayload, Variety } from "@/types/varieties";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { updateVariety } from "@/api/varieties";
import { useForm } from "@/forms";

const { t } = useI18n();

const props = defineProps<{
  variety: Variety;
}>();

const emit = defineEmits<{
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
const summary = ref<string>("");

const hasChanges = computed<boolean>(() =>
  Boolean(
    props.variety.key !== key.value ||
    (props.variety.name ?? "") !== name.value ||
    (props.variety.summary ?? "") !== summary.value ||
    (props.variety.content ?? "") !== content.value ||
    props.variety.isDefault !== isDefault.value ||
    props.variety.canChangeForm !== canChangeForm.value ||
    (props.variety.genderRatio ?? null) !== genderRatio.value ||
    (props.variety.genus ?? "") !== genus.value,
  ),
);

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    try {
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
      const variety: Variety = await updateVariety(props.variety.id, payload);
      reinitialize();
      emit("updated", variety);
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

watch(
  () => props.variety,
  (variety) => {
    key.value = variety.key;
    name.value = variety.name ?? "";
    summary.value = variety.summary ?? "";
    content.value = variety.content ?? "";
    isDefault.value = variety.isDefault;
    canChangeForm.value = variety.canChangeForm;
    genderRatio.value = variety.genderRatio ?? null;
    genus.value = variety.genus ?? "";
  },
  { deep: true, immediate: true },
);
</script>
