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
        :icon="ability ? 'fas fa-floppy-disk' : 'fas fa-plus'"
        :loading="isLoading"
        :text="ability ? 'actions.save' : 'actions.create'"
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
import type { Ability, CreateOrReplaceAbilityPayload, UpdateAbilityPayload } from "@/types/abilities";
import type { ApiFailure, ProblemDetails } from "@/types/api";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createAbility, updateAbility } from "@/api/abilities";
import { useForm } from "@/forms";

const { slugify } = stringUtils;

const props = defineProps<{
  ability?: Ability;
}>();

const emit = defineEmits<{
  (e: "created", value: Ability): void;
  (e: "error", value: unknown): void;
  (e: "updated", value: Ability): void;
}>();

const content = ref<string>("");
const isLoading = ref<boolean>(false);
const key = ref<string>("");
const keyAlreadyUsed = ref<boolean>(false);
const keyField = ref<InstanceType<typeof KeyField> | null>(null);
const name = ref<string>("");
const summary = ref<string>("");

const hasChanges = computed<boolean>(() => {
  const ability: Ability | undefined = props.ability;
  return (
    key.value !== (ability?.key ?? "") ||
    name.value !== (ability?.name ?? "") ||
    summary.value !== (ability?.summary ?? "") ||
    content.value !== (ability?.content ?? "")
  );
});

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    try {
      if (props.ability) {
        const payload: UpdateAbilityPayload = {
          key: key.value,
          name: { value: name.value },
          summary: { value: summary.value },
          content: { value: content.value },
        };
        const updated: Ability = await updateAbility(props.ability.id, payload);
        emit("updated", updated);
      } else {
        const payload: CreateOrReplaceAbilityPayload = {
          key: key.value,
          name: name.value,
          summary: summary.value,
          content: content.value,
        };
        const created: Ability = await createAbility(payload);
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
  () => props.ability,
  (ability) => {
    key.value = ability?.key ?? "";
    name.value = ability?.name ?? "";
    summary.value = ability?.summary ?? "";
    content.value = ability?.content ?? "";
  },
  { deep: true, immediate: true },
);
</script>
