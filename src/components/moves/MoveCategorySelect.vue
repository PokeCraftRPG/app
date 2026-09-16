<template>
  <TarSelect
    floating
    :id="id"
    :label="label ? t(label) : undefined"
    :model-value="modelValue"
    :options="options"
    :placeholder="placeholder ? t(placeholder) : undefined"
    @update:model-value="$emit('update:model-value', $event ? ($event as MoveCategory) : '')"
  >
    <template v-if="modelValue" #append>
      <span class="input-group-text">
        <MoveCategoryIcon :category="modelValue" height="32" />
      </span>
    </template>
  </TarSelect>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import MoveCategoryIcon from "./MoveCategoryIcon.vue";
import TarSelect from "@/components/tar/TarSelect.vue";
import type { MoveCategory } from "@/types/moves";
import type { SelectOption } from "@/types/tar/select";

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: MoveCategory | "";
    placeholder?: string;
  }>(),
  {
    id: "category",
    label: "moves.category.label",
    placeholder: "all",
  },
);

defineEmits<{
  (e: "update:model-value", value: MoveCategory | ""): void;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("moves.category.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);
</script>
