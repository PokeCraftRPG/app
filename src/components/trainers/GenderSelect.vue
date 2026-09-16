<template>
  <TarSelect
    floating
    :id="id"
    :label="label ? t(label) : undefined"
    :model-value="modelValue"
    :options="options"
    :placeholder="placeholder ? t(placeholder) : undefined"
    @update:model-value="$emit('update:model-value', $event ? ($event as Gender) : '')"
  >
    <template #append>
      <span class="input-group-text">
        <GenderIcon :gender="modelValue || undefined" />
      </span>
    </template>
  </TarSelect>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import GenderIcon from "@/components/shared/GenderIcon.vue";
import TarSelect from "@/components/tar/TarSelect.vue";
import type { Gender } from "@/types/trainers";
import type { SelectOption } from "@/types/tar/select";

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: Gender | "";
    placeholder?: string;
  }>(),
  {
    id: "gender",
    label: "trainers.gender.label",
    placeholder: "all",
  },
);

defineEmits<{
  (e: "update:model-value", value: Gender | ""): void;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("trainers.gender.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);
</script>
