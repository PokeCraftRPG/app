<template>
  <SelectField
    :id="id"
    :label="label ? t(label) : undefined"
    :model-value="modelValue?.toString() ?? ''"
    :options="options"
    :placeholder="placeholder ? t(placeholder) : undefined"
    :required="required"
    @update:model-value="$emit('update:model-value', $event === '' ? null : (parseNumber($event) ?? null))"
  />
</template>

<script setup lang="ts">
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import SelectField from "@/components/forms/SelectField.vue";
import type { SelectOption } from "@/types/tar/select";

const { orderBy } = arrayUtils;
const { parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: number | null;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "gender-ratio",
    label: "varieties.genderRatio.label",
    placeholder: "varieties.genderRatio.placeholder",
  },
);

defineEmits<{
  (e: "update:model-value", value: number | null): void;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("varieties.genderRatio.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "value",
  ),
);
</script>
