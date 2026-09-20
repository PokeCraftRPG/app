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
import type { VarietySummary } from "@/types/varieties";
import type { SelectOption } from "@/types/tar/select";
import { formatVariety } from "@/utils/format";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    varieties?: VarietySummary[];
  }>(),
  {
    id: "variety",
    label: "varieties.label",
    placeholder: "all",
    varieties: () => [],
  },
);

const emit = defineEmits<{
  (e: "selected", value: VarietySummary | undefined): void;
  (e: "update:model-value", value: string): void;
}>();

const options = computed<SelectOption[]>(() => props.varieties.map((variety) => ({ text: formatVariety(variety), value: variety.id })));

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);
  emit(
    "selected",
    props.varieties.find((variety) => variety.id === id),
  );
}
</script>
