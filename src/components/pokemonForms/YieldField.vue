<template>
  <InputField
    :id="id"
    :label="label"
    :max="max"
    :min="min"
    :model-value="modelValue?.toString()"
    :required="!statistic"
    :step="step"
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

const props = withDefaults(
  defineProps<{
    modelValue?: number | string;
    statistic?: PokemonStatistic;
    step?: number | string;
  }>(),
  {
    step: 1,
  },
);

defineEmits<{
  (e: "update:model-value", value: number | undefined): void;
}>();

const id = computed<string>(() => (props.statistic ? `yield-${props.statistic?.toLowerCase()}` : "yield-experience"));
const label = computed<string>(() => (props.statistic ? t(`pokemon.statistic.options.${props.statistic}`) : t("pokemon.experience.label")));
const max = computed<number>(() => (props.statistic ? 3 : 999));
const min = computed<number>(() => (props.statistic ? 0 : 1));
</script>
