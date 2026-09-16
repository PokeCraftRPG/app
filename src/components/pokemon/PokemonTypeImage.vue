<template>
  <TarImage :alt="alt" :height="height" :src="src" />
</template>

<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import TarImage from "@/components/tar/TarImage.vue";
import type { PokemonType } from "@/types/pokemon";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  height?: number | string;
  tera?: boolean | string;
  type: PokemonType;
}>();

const alt = computed<string>(() => t(`pokemon.type.options.${props.type}`));
const src = computed<string>(() =>
  parseBoolean(props.tera) ? `/img/types/tera/${props.type.toLowerCase()}.png` : `/img/types/${props.type.toLowerCase()}.png`,
);
</script>
