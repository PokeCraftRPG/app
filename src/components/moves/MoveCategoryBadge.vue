<template>
  <span><MoveCategoryIcon :category="category" :height="height" />&nbsp;{{ text }}</span>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import MoveCategoryIcon from "./MoveCategoryIcon.vue";
import type { MoveCategory } from "@/types/moves";

const { t } = useI18n();

const props = defineProps<{
  category: MoveCategory;
  height?: number | string;
}>();

const text = computed<string>(() => t(`moves.category.options.${props.category}`));
</script>
