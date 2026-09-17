<template>
  <TarBadge variant="secondary"><font-awesome-icon icon="fas fa-check" aria-hidden="true" />&nbsp;{{ t("varieties.default.label") }}</TarBadge>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import TarBadge from "@/components/tar/TarBadge.vue";

const { t } = useI18n();
</script>
