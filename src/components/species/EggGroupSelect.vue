<template>
  <TarSelect
    floating
    :id="id"
    :label="label ? t(label) : undefined"
    :model-value="modelValue"
    :options="options"
    :placeholder="placeholder ? t(placeholder) : undefined"
    @update:model-value="$emit('update:model-value', $event ? ($event as EggGroup) : '')"
  />
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import TarSelect from "@/components/tar/TarSelect.vue";
import type { EggGroup } from "@/types/species";
import type { SelectOption } from "@/types/tar/select";

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: EggGroup | "";
    placeholder?: string;
  }>(),
  {
    id: "egg",
    label: "species.egg.group.label",
    placeholder: "all",
  },
);

defineEmits<{
  (e: "update:model-value", value: EggGroup | ""): void;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("species.egg.group.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);
</script>
