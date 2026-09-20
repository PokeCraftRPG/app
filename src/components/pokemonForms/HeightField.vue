<template>
  <InputField
    :id="id"
    :label="label ? t(label) : undefined"
    :max="max"
    :min="min"
    :model-value="modelValue?.toString() ?? ''"
    :required="required"
    :step="step"
    type="number"
    @update:model-value="$emit('update:model-value', parseNumber($event))"
  >
    <template #append>
      <span class="input-group-text">{{ t("unit.meter") }}</span>
    </template>
  </InputField>
</template>

<script setup lang="ts">
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import InputField from "@/components/forms/InputField.vue";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    min?: number | string;
    modelValue?: number | string;
    required?: boolean | string;
    step?: number | string;
  }>(),
  {
    id: "height",
    label: "forms.size.height",
    max: 999.9,
    min: 0,
    step: 0.1,
  },
);

defineEmits<{
  (e: "update:model-value", value: number | undefined): void;
}>();
</script>
