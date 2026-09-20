<template>
  <TarSelect
    :disabled="!options.length"
    floating
    :id="id"
    :label="label ? t(label) : undefined"
    :model-value="modelValue"
    :options="options"
    :placeholder="placeholder ? t(placeholder) : undefined"
    @update:model-value="onModelValueUpdate($event ?? '')"
  />
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import TarSelect from "@/components/tar/TarSelect.vue";
import type { AbilitySummary } from "@/types/abilities";
import type { SelectOption } from "@/types/tar/select";
import { formatAbility } from "@/utils/format";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    abilities?: AbilitySummary[];
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
  }>(),
  {
    abilities: () => [],
    id: "ability",
    label: "abilities.label",
    placeholder: "all",
  },
);

const emit = defineEmits<{
  (e: "selected", value: AbilitySummary | undefined): void;
  (e: "update:model-value", value: string): void;
}>();

const options = computed<SelectOption[]>(() => props.abilities.map((ability) => ({ text: formatAbility(ability), value: ability.id })));

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);
  emit(
    "selected",
    props.abilities.find((ability) => ability.id === id),
  );
}
</script>
