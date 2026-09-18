<template>
  <SelectField
    :disabled="disabled"
    :id="id"
    :label="label ? t(label) : undefined"
    :model-value="modelValue"
    :options="options"
    :placeholder="placeholder ? t(placeholder) : undefined"
    :required="required"
    @update:model-value="$emit('update:model-value', $event ? ($event as SpeciesCategory) : '')"
  />
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import SelectField from "@/components/forms/SelectField.vue";
import type { SpeciesCategory } from "@/types/species";
import type { SelectOption } from "@/types/tar/select";

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

withDefaults(
  defineProps<{
    disabled?: boolean | string;
    id?: string;
    label?: string;
    modelValue?: SpeciesCategory | "";
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "category",
    label: "species.category.label",
    placeholder: "species.category.placeholder",
  },
);

defineEmits<{
  (e: "update:model-value", value: SpeciesCategory | ""): void;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("species.category.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);
</script>
