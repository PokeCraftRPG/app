<template>
  <SelectField
    :disabled="!options.length"
    :id="id"
    :label="label ? t(label) : undefined"
    :model-value="modelValue"
    :options="options"
    :placeholder="placeholder ? t(placeholder) : undefined"
    :required="required"
    @update:model-value="onModelValueUpdate($event ?? '')"
  />
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import SelectField from "@/components/forms/SelectField.vue";
import type { AbilitySummary } from "@/types/abilities";
import type { SelectOption } from "@/types/tar/select";
import { formatAbility } from "@/utils/format";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    abilities?: AbilitySummary[];
    exclude?: string[];
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    abilities: () => [],
    exclude: () => [],
    id: "ability",
    label: "abilities.label",
    placeholder: "abilities.placeholder",
  },
);

const emit = defineEmits<{
  (e: "selected", value: AbilitySummary | undefined): void;
  (e: "update:model-value", value: string): void;
}>();

const options = computed<SelectOption[]>(() => {
  const excluded = new Set(props.exclude);
  return props.abilities.filter((ability) => !excluded.has(ability.id)).map((ability) => ({ text: formatAbility(ability), value: ability.id }));
});

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);
  emit(
    "selected",
    props.abilities.find((ability) => ability.id === id),
  );
}
</script>
