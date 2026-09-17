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
import type { SpeciesSummary } from "@/types/species";
import type { SelectOption } from "@/types/tar/select";
import { formatSpecies } from "@/utils/format";

const { n, t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
    species?: SpeciesSummary[];
  }>(),
  {
    id: "species",
    label: "species.label",
    placeholder: "species.placeholder",
    species: () => [],
  },
);

const emit = defineEmits<{
  (e: "selected", value: SpeciesSummary | undefined): void;
  (e: "update:model-value", value: string): void;
}>();

const options = computed<SelectOption[]>(() => props.species.map((species) => ({ text: formatSpecies(species, n), value: species.id })));

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);
  emit(
    "selected",
    props.species.find((species) => species.id === id),
  );
}
</script>
