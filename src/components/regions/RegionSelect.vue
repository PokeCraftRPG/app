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
import type { SelectOption } from "@/types/tar/select";
import type { RegionSummary } from "@/types/regions";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    regions?: RegionSummary[];
  }>(),
  {
    id: "region",
    label: "regions.label",
    placeholder: "all",
    regions: () => [],
  },
);

const emit = defineEmits<{
  (e: "selected", value: RegionSummary | undefined): void;
  (e: "update:model-value", value: string): void;
}>();

const options = computed<SelectOption[]>(() => props.regions.map((region) => ({ text: region.name ?? region.key, value: region.id })));

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);

  const region: RegionSummary | undefined = props.regions.find((region) => region.id === id);
  emit("selected", region);
}
</script>
