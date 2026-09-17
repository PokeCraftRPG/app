<template>
  <TarSelect
    floating
    :id="id"
    :label="label ? t(label) : undefined"
    :model-value="modelValue"
    :options="options"
    :placeholder="placeholder ? t(placeholder) : undefined"
    @update:model-value="$emit('update:model-value', $event ? ($event as GrowthRate) : '')"
  />
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import TarSelect from "@/components/tar/TarSelect.vue";
import type { GrowthRate } from "@/types/species";
import type { SelectOption } from "@/types/tar/select";

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: GrowthRate | "";
    placeholder?: string;
  }>(),
  {
    id: "growth",
    label: "species.growthRate.label",
    placeholder: "all",
  },
);

defineEmits<{
  (e: "update:model-value", value: GrowthRate | ""): void;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("species.growthRate.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);
</script>
