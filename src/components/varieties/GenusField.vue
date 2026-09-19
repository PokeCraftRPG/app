<template>
  <InputField
    :id="id"
    :label="label ? t(label) : undefined"
    :max="max"
    :model-value="modelValue"
    :required="required"
    @update:model-value="$emit('update:model-value', $event ?? '')"
  />
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import InputField from "@/components/forms/InputField.vue";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    modelValue?: string;
    required?: boolean | string;
  }>(),
  {
    id: "genus",
    label: "varieties.genus",
    max: 16,
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>
