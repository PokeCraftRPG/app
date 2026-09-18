<template>
  <form class="border-top border-secondary-subtle pt-4" @submit.prevent="handleSubmit(submit)">
    <KeyAlreadyUsed v-model="keyAlreadyUsed" />
    <ItemCategoryField class="mb-3" :disabled="Boolean(item)" required v-model="category" />
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
        <PriceField class="mb-3" v-model="price" />
      </div>
      <div class="col-md-6">
        <WeightField class="mb-3" v-model="weight" />
      </div>
    </div>
    <SummaryField class="mb-3" v-model="summary" />
    <ContentField class="mb-3" v-model="content" />
    <div class="d-flex justify-content-end mb-3">
      <SaveButton
        :disabled="!hasChanges || isLoading"
        :icon="item ? 'fas fa-floppy-disk' : 'fas fa-plus'"
        :loading="isLoading"
        :text="item ? 'actions.save' : 'actions.create'"
      />
    </div>
  </form>
</template>

<script setup lang="ts">
import { computed, nextTick, ref, watch } from "vue";
import { stringUtils } from "logitar-js";

import ContentField from "@/components/shared/ContentField.vue";
import ItemCategoryField from "./ItemCategoryField.vue";
import KeyAlreadyUsed from "@/components/shared/KeyAlreadyUsed.vue";
import KeyField from "@/components/shared/KeyField.vue";
import NameField from "@/components/shared/NameField.vue";
import PriceField from "./PriceField.vue";
import SaveButton from "@/components/shared/SaveButton.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import WeightField from "./WeightField.vue";
import type { ApiFailure, ProblemDetails } from "@/types/api";
import type { CreateOrReplaceItemPayload, Item, ItemCategory, UpdateItemPayload } from "@/types/items";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createItem, updateItem } from "@/api/items";
import { fromHundredths, toHundredths } from "@/utils/number";
import { useForm } from "@/forms";

const { slugify } = stringUtils;

const props = defineProps<{
  item?: Item;
}>();

const emit = defineEmits<{
  (e: "created", value: Item): void;
  (e: "error", value: unknown): void;
  (e: "updated", value: Item): void;
}>();

const category = ref<ItemCategory | "">("");
const content = ref<string>("");
const isLoading = ref<boolean>(false);
const key = ref<string>("");
const keyAlreadyUsed = ref<boolean>(false);
const keyField = ref<InstanceType<typeof KeyField> | null>(null);
const name = ref<string>("");
const price = ref<number>();
const summary = ref<string>("");
const weight = ref<number>();

const hasChanges = computed<boolean>(() => {
  const item: Item | undefined = props.item;
  return (
    category.value !== (item?.category ?? "") ||
    key.value !== (item?.key ?? "") ||
    name.value !== (item?.name ?? "") ||
    summary.value !== (item?.summary ?? "") ||
    content.value !== (item?.content ?? "") ||
    (price.value ?? 0) !== (fromHundredths(item?.price) ?? 0) ||
    (weight.value ?? 0) !== (fromHundredths(item?.weight) ?? 0)
  );
});

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    try {
      if (props.item) {
        const payload: UpdateItemPayload = {
          key: key.value,
          name: { value: name.value },
          summary: { value: summary.value },
          content: { value: content.value },
          price: { value: toHundredths(price.value) || null },
          weight: { value: toHundredths(weight.value) || null },
        };
        const updated: Item = await updateItem(props.item.id, payload);
        emit("updated", updated);
      } else if (category.value) {
        const payload: CreateOrReplaceItemPayload = {
          category: category.value,
          key: key.value,
          name: name.value,
          summary: summary.value,
          content: content.value,
          price: toHundredths(price.value) || null,
          weight: toHundredths(weight.value) || null,
        };
        const created: Item = await createItem(payload);
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
  () => props.item,
  (item) => {
    category.value = item?.category ?? "";
    key.value = item?.key ?? "";
    name.value = item?.name ?? "";
    summary.value = item?.summary ?? "";
    content.value = item?.content ?? "";
    price.value = fromHundredths(item?.price);
    weight.value = fromHundredths(item?.weight);
  },
  { deep: true, immediate: true },
);
</script>
