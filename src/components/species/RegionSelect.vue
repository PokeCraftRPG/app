<template>
  <TarSelect
    :disabled="!options.length"
    floating
    :id="id"
    :label="label ? t(label) : undefined"
    :model-value="modelValue"
    :options="options"
    :placeholder="placeholder ? t(placeholder) : undefined"
    @update:model-value="$emit('update:model-value', $event ?? '')"
  />
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import TarSelect from "@/components/tar/TarSelect.vue";
import type { FilterOption } from "@/types/search";
import type { SelectOption } from "@/types/tar/select";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    regions?: FilterOption[];
  }>(),
  {
    id: "region",
    label: "regions.label",
    placeholder: "all",
    regions: () => [],
  },
);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

const options = computed<SelectOption[]>(() => props.regions.map((region) => ({ text: region.text, value: region.value })));
</script>
