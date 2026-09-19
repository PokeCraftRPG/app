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
import type { MoveSummary } from "@/types/moves";
import type { SelectOption } from "@/types/tar/select";
import { formatMove } from "@/utils/format";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    moves?: MoveSummary[];
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "move",
    label: "moves.label",
    placeholder: "moves.placeholder",
    moves: () => [],
  },
);

const emit = defineEmits<{
  (e: "selected", value: MoveSummary | undefined): void;
  (e: "update:model-value", value: string): void;
}>();

const options = computed<SelectOption[]>(() => props.moves.map((move) => ({ text: formatMove(move), value: move.id })));

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);
  emit(
    "selected",
    props.moves.find((move) => move.id === id),
  );
}
</script>
