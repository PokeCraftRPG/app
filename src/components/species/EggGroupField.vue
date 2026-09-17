<template>
  <SelectField
    :disabled="disabled"
    :id="id"
    :label="label ? t(label) : undefined"
    :model-value="modelValue"
    :options="options"
    :placeholder="placeholder ? t(placeholder) : undefined"
    :required="required"
    @update:model-value="$emit('update:model-value', $event ? ($event as EggGroup) : '')"
  />
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import SelectField from "@/components/forms/SelectField.vue";
import type { EggGroup } from "@/types/species";
import type { SelectOption } from "@/types/tar/select";

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

const props = withDefaults(
  defineProps<{
    disabled?: boolean | string;
    exclude?: EggGroup[];
    id?: string;
    label?: string;
    modelValue?: EggGroup | "";
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    exclude: () => [],
    id: "egg-group",
    label: "species.egg.group.label",
    placeholder: "species.egg.group.placeholder",
  },
);

defineEmits<{
  (e: "update:model-value", value: EggGroup | ""): void;
}>();

const options = computed<SelectOption[]>(() => {
  const excluded = new Set(props.exclude);
  return orderBy(
    Object.entries(tm(rt("species.egg.group.options")))
      .filter(([value]) => !excluded.has(value as EggGroup))
      .map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  );
});
</script>
