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
import type { SelectOption } from "@/types/tar/select";
import type { RegionSummary } from "@/types/regions";
import { formatRegion } from "@/utils/format";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    regions?: RegionSummary[];
    required?: boolean | string;
  }>(),
  {
    id: "region",
    label: "regions.label",
    placeholder: "regions.placeholder",
    regions: () => [],
  },
);

const emit = defineEmits<{
  (e: "selected", value: RegionSummary | undefined): void;
  (e: "update:model-value", value: string): void;
}>();

const options = computed<SelectOption[]>(() => props.regions.map((region) => ({ text: formatRegion(region), value: region.id })));

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);
  emit(
    "selected",
    props.regions.find((region) => region.id === id),
  );
}
</script>
