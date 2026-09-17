<template>
  <TarSelect
    floating
    :id="id"
    :label="label ? t(label) : undefined"
    :model-value="modelValue?.toString()"
    :options="options"
    :placeholder="placeholder ? t(placeholder) : undefined"
    @update:model-value="$emit('update:model-value', parseBoolean($event))"
  />
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import TarSelect from "@/components/tar/TarSelect.vue";
import type { SelectOption } from "@/types/tar/select";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: boolean | string;
    placeholder?: string;
  }>(),
  {
    id: "boolean",
    placeholder: "all",
  },
);

defineEmits<{
  (e: "update:model-value", value: boolean | undefined): void;
}>();

const options = computed<SelectOption[]>(() => [
  { text: t("yes"), value: "true" },
  { text: t("no"), value: "false" },
]);
</script>
