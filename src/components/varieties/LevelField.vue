<template>
  <InputField
    :id="id"
    :label="label ? t(label) : undefined"
    :max="max"
    :min="min"
    :model-value="modelValue?.toString() ?? ''"
    ref="inputRef"
    :required="required"
    :step="step"
    type="number"
    @update:model-value="$emit('update:model-value', parseNumber($event) ?? 0)"
  />
</template>

<script setup lang="ts">
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { ref } from "vue";

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
    id: "level",
    label: "varieties.level.label",
    max: 100,
    min: 1,
    step: 1,
  },
);

defineEmits<{
  (e: "update:model-value", value: number): void;
}>();

const inputRef = ref<InstanceType<typeof InputField> | null>();

function focus(): void {
  inputRef.value?.focus();
}
defineExpose({ focus });
</script>
