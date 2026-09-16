<template>
  <TarSelect floating :id="id" :label="t(label)" :model-value="modelValue" :options="options" @update:model-value="$emit('update:model-value', $event ?? '')">
    <template #append>
      <TarButton :icon="icon" outline variant="secondary" @click="$emit('descending', !isDescending)" />
    </template>
  </TarSelect>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
import TarSelect from "@/components/tar/TarSelect.vue";
import type { SelectOption } from "@/types/tar/select";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    descending?: boolean | string;
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
  (e: "descending", value: boolean): void;
  (e: "update:model-value", value: string): void;
}>();

const isDescending = computed<boolean>(() => parseBoolean(props.descending) ?? false);
const icon = computed<string>(() => (isDescending.value ? "fas fa-arrow-down-long" : "fas fa-arrow-up-long"));
</script>
