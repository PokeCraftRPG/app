<template>
  <TarButton
    :disabled="disabled"
    :icon="icon"
    :loading="loading"
    :size="size"
    :status="status ? t(status) : undefined"
    :text="text ? t(text) : undefined"
    :type="type"
  />
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import TarButton from "@/components/tar/TarButton.vue";
import type { ButtonSize, ButtonType } from "@/types/tar/button";

const { t } = useI18n();

withDefaults(
  defineProps<{
    disabled?: boolean | string;
    icon?: string;
    loading?: boolean | string;
    size?: ButtonSize;
    status?: string;
    text?: string;
    type?: ButtonType;
  }>(),
  {
    icon: "fas fa-floppy-disk",
    size: "large",
    status: "loading",
    text: "actions.save",
    type: "submit",
  },
);
</script>
