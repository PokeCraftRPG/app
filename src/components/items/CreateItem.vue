<template>
  <div>
    <TarButton icon="fas fa-plus" size="large" :text="t('actions.create')" @click="open" />
    <TarModal centered :close="t('actions.close')" fade scrollable ref="modal" :title="t('items.create')">
      <KeyAlreadyUsed v-model="keyAlreadyUsed" />
      <form @submit.prevent="handleSubmit(submit)">
        <NameField class="mb-3" :model-value="name" required @update:model-value="updateName" />
        <KeyField class="mb-3" ref="keyField" required v-model="key" />
        <ItemCategoryField class="mb-3" required v-model="category" />
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
import { ref } from "vue";
import { stringUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import ItemCategoryField from "./ItemCategoryField.vue";
import KeyAlreadyUsed from "@/components/shared/KeyAlreadyUsed.vue";
import KeyField from "@/components/shared/KeyField.vue";
import NameField from "@/components/shared/NameField.vue";
import TarButton from "@/components/tar/TarButton.vue";
import TarModal from "@/components/tar/TarModal.vue";
import type { ApiFailure, ProblemDetails } from "@/types/api";
import type { CreateOrReplaceItemPayload, Item, ItemCategory } from "@/types/items";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createItem } from "@/api/items";
import { useForm } from "@/forms";

const { slugify } = stringUtils;
const { t } = useI18n();

const emit = defineEmits<{
  (e: "created", value: Item): void;
  (e: "error", value: unknown): void;
}>();

const category = ref<ItemCategory | "">("");
const isLoading = ref<boolean>(false);
const key = ref<string>("");
const keyAlreadyUsed = ref<boolean>(false);
const keyField = ref<InstanceType<typeof KeyField> | null>(null);
const modal = ref<InstanceType<typeof TarModal> | null>(null);
const name = ref<string>("");

function clear(): void {
  reset();
  keyAlreadyUsed.value = false;
}

function cancel(): void {
  clear();
  modal.value?.hide();
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
  if (!isLoading.value && category.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    try {
      const payload: CreateOrReplaceItemPayload = {
        category: category.value,
        key: key.value,
        name: name.value,
      };
      const item: Item = await createItem(payload);
      clear();
      modal.value?.hide();
      emit("created", item);
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
</script>
