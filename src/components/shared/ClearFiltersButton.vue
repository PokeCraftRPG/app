<template>
  <TarButton icon="fas fa-arrow-rotate-left" outline :text="t('filters.clear')" variant="secondary" @click="$emit('click')" />
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
const { t } = useI18n();

defineEmits<{
  (e: "click"): void;
}>();
</script>
