<template>
  <SelectField
    :disabled="disabled"
    :id="id"
    :label="label ? t(label) : undefined"
    :model-value="modelValue"
    :options="options"
    :placeholder="placeholder ? t(placeholder) : undefined"
    :required="required"
    @update:model-value="$emit('update:model-value', $event ? ($event as PokemonType) : '')"
  >
    <template v-if="modelValue" #append>
      <span class="input-group-text">
        <PokemonTypeImage height="32" :type="modelValue" />
      </span>
    </template>
  </SelectField>
</template>

<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import SelectField from "@/components/forms/SelectField.vue";
import PokemonTypeImage from "./PokemonTypeImage.vue";
import type { PokemonType } from "@/types/pokemon";
import type { SelectOption } from "@/types/tar/select";

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

withDefaults(
  defineProps<{
    disabled?: boolean | string;
    id?: string;
    label?: string;
    modelValue?: PokemonType | "";
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "type",
    label: "pokemon.type.label",
    placeholder: "pokemon.type.placeholder",
  },
);

defineEmits<{
  (e: "update:model-value", value: PokemonType | ""): void;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("pokemon.type.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);
</script>
