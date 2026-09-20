<template>
  <InputField
    :id="id"
    :label="label"
    :max="255"
    :min="1"
    :model-value="modelValue?.toString() ?? ''"
    required
    :step="1"
    type="number"
    @update:model-value="$emit('update:model-value', parseNumber($event))"
  />
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import InputField from "@/components/forms/InputField.vue";
import type { PokemonStatistic } from "@/types/pokemon";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  modelValue?: number | string;
  statistic: PokemonStatistic;
}>();

defineEmits<{
  (e: "update:model-value", value: number | undefined): void;
}>();

const id = computed<string>(() => `base-${props.statistic.toLowerCase()}`);
const label = computed<string>(() => t(`pokemon.statistic.options.${props.statistic}`));
</script>
