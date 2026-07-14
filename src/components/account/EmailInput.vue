<template>
  <TarInput
    floating
    :id="id"
    :label="t(label)"
    :max="max"
    :model-value="modelValue"
    :placeholder="t(label)"
    :required="required"
    :type="type"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import TarInput from "@/components/tar/TarInput.vue";
import type { InputType } from "@/types/tar/input";

const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    max?: number | string;
    modelValue?: string;
    required?: boolean | string;
    type?: InputType;
  }>(),
  {
    id: "email",
    label: "account.email.address",
    max: 255,
    type: "email",
  },
);

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>
