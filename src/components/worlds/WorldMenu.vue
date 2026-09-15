<template>
  <div class="text-center text-body-secondary py-5">
    <p class="mb-0">{{ t("worlds.portal.comingSoon") }}</p>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

const { t } = useI18n();
</script>
