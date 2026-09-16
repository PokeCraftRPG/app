<template>
  <TarImage :alt="alt" :height="height" :src="src" />
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import TarImage from "@/components/tar/TarImage.vue";
import type { MoveCategory } from "@/types/moves";

const { t } = useI18n();

const props = defineProps<{
  category: MoveCategory;
  height?: number | string;
}>();

const alt = computed<string>(() => t(`moves.category.options.${props.category}`));
const src = computed<string>(() => `/img/moves/${props.category.toLowerCase()}.png`);
</script>
