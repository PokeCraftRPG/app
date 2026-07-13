<template>
  <main class="container">
    <h1>{{ title }}</h1>
    <AppBreadcrumb />
    <p>
      A multi-tenant game Web application managing world-scoped data, character sheets, and real-time gameplay state for interactive Pokémon roleplaying
      experiences.
    </p>
    <!-- TODO(fpion): should the home page display breadcrumbs? -->
    <!-- TODO(fpion): should the home page display a title? -->
  </main>
</template>

<script setup lang="ts">
import { computed, onMounted } from "vue";
import { useI18n } from "vue-i18n";

import AppBreadcrumb from "@/components/shared/AppBreadcrumb.vue";
import { useDocument } from "@/composables/document";

const document = useDocument();
const { t } = useI18n();

const title = computed<string>(() => t("home"));

onMounted(() => document.setTitle(title.value));
</script>
