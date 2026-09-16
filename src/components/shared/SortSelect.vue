<template>
  <TarSelect floating :id="id" :label="t(label)" :model-value="modelValue" :options="options" @update:model-value="$emit('update:model-value', $event ?? '')">
    <template #append>
      <TarButton :icon="icon" outline variant="secondary" @click="$emit('update:direction', direction === 'Ascending' ? 'Descending' : 'Ascending')" />
    </template>
  </TarSelect>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
import TarSelect from "@/components/tar/TarSelect.vue";
import type { SelectOption } from "@/types/tar/select";
import type { SortDirection } from "@/types/search";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    direction?: string;
    id?: string;
    label?: string;
    modelValue?: string;
    options?: SelectOption[];
  }>(),
  {
    id: "sort",
    label: "sort",
  },
);

defineEmits<{
  (e: "update:direction", value: SortDirection): void;
  (e: "update:model-value", value: string): void;
}>();

const icon = computed<string>(() => {
  switch (props.direction) {
    case "Ascending":
      return "fas fa-arrow-up-long";
    case "Descending":
      return "fas fa-arrow-down-long";
    default:
      return "";
  }
});
</script>
