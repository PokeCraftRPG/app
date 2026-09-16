<template>
  <TarButton
    :disabled="loading"
    icon="fas fa-arrows-rotate"
    :loading="loading"
    :status="t('loading')"
    :text="t('actions.refresh')"
    variant="secondary"
    @click="$emit('click')"
  />
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";

const { t } = useI18n();

defineProps<{
  loading?: boolean | string;
}>();

defineEmits<{
  (e: "click"): void;
}>();
</script>
