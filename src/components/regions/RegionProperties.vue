<template>
  <form class="border-top border-secondary-subtle pt-4" @submit.prevent="handleSubmit(submit)">
    <KeyAlreadyUsed v-model="keyAlreadyUsed" />
    <div class="row">
      <div class="col-lg-6">
        <NameField class="mb-3" v-model="name" />
      </div>
      <div class="col-lg-6">
        <KeyField class="mb-3" ref="keyField" required v-model="key" />
      </div>
    </div>
    <SummaryField class="mb-3" v-model="summary" />
    <ContentField class="mb-3" v-model="content" />
    <div class="d-flex justify-content-end mb-3">
      <SaveButton
        :disabled="!hasChanges || isLoading"
        :icon="region ? 'fas fa-floppy-disk' : 'fas fa-plus'"
        :loading="isLoading"
        :text="region ? 'actions.save' : 'actions.create'"
      />
    </div>
  </form>
</template>

<script setup lang="ts">
import { computed, nextTick, ref, watch } from "vue";
import { stringUtils } from "logitar-js";

import ContentField from "@/components/shared/ContentField.vue";
import KeyAlreadyUsed from "@/components/shared/KeyAlreadyUsed.vue";
import KeyField from "@/components/shared/KeyField.vue";
import NameField from "@/components/shared/NameField.vue";
import SaveButton from "@/components/shared/SaveButton.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import type { ApiFailure, ProblemDetails } from "@/types/api";
import type { CreateOrReplaceRegionPayload, Region, UpdateRegionPayload } from "@/types/regions";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createRegion, updateRegion } from "@/api/regions";
import { useForm } from "@/forms";

const { slugify } = stringUtils;

const props = defineProps<{
  region?: Region;
}>();

const emit = defineEmits<{
  (e: "created", value: Region): void;
  (e: "error", value: unknown): void;
  (e: "updated", value: Region): void;
}>();

const content = ref<string>("");
const isLoading = ref<boolean>(false);
const key = ref<string>("");
const keyAlreadyUsed = ref<boolean>(false);
const keyField = ref<InstanceType<typeof KeyField> | null>(null);
const name = ref<string>("");
const summary = ref<string>("");

const hasChanges = computed<boolean>(() => {
  const region: Region | undefined = props.region;
  return (
    key.value !== (region?.key ?? "") ||
    name.value !== (region?.name ?? "") ||
    summary.value !== (region?.summary ?? "") ||
    content.value !== (region?.content ?? "")
  );
});

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    try {
      if (props.region) {
        const payload: UpdateRegionPayload = {
          key: key.value,
          name: { value: name.value },
          summary: { value: summary.value },
          content: { value: content.value },
        };
        const updated: Region = await updateRegion(props.region.id, payload);
        emit("updated", updated);
      } else {
        const payload: CreateOrReplaceRegionPayload = {
          key: key.value,
          name: name.value,
          summary: summary.value,
          content: content.value,
        };
        const created: Region = await createRegion(payload);
        emit("created", created);
      }
      nextTick(reinitialize);
    } catch (e: unknown) {
      const failure = e as ApiFailure;
      if (failure.status === StatusCodes.Conflict) {
        const problemDetails = failure.data as ProblemDetails;
        if (problemDetails.error && problemDetails.error.code === ErrorCodes.KeyAlreadyUsed) {
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

watch(name, (newValue, oldValue) => {
  if (!key.value || key.value === slugify(oldValue)) {
    key.value = slugify(newValue);
  }
});
watch(
  () => props.region,
  (region) => {
    key.value = region?.key ?? "";
    name.value = region?.name ?? "";
    summary.value = region?.summary ?? "";
    content.value = region?.content ?? "";
  },
  { deep: true, immediate: true },
);
</script>
