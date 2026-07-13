<template>
  <nav class="navbar navbar-expand-lg bg-body-tertiary" data-bs-theme="dark">
    <div class="container-fluid">
      <RouterLink :to="{ name: 'Home' }" class="navbar-brand">
        <img src="@/assets/img/logo.png" :alt="`${t('brand')} Logo`" height="32" />
        {{ t("brand") }}
        <TarBadge v-if="environment !== 'production'" variant="warning">{{ environment }}</TarBadge>
      </RouterLink>
      <button
        class="navbar-toggler"
        type="button"
        data-bs-toggle="collapse"
        data-bs-target="#navbarSupportedContent"
        aria-controls="navbarSupportedContent"
        aria-expanded="false"
        aria-label="Toggle navigation"
      >
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse" id="navbarSupportedContent">
        <ul class="navbar-nav me-auto mb-2 mb-lg-0">
          <!-- TODO(fpion): <li class="nav-item">
            <RouterLink :to="{ name: 'Countries' }" class="nav-link"><font-awesome-icon icon="fas fa-globe" />&nbsp;{{ t("countries.title") }}</RouterLink>
          </li> -->
        </ul>
        <ul class="navbar-nav mb-2 mb-lg-0">
          <li class="nav-item">
            <!-- TODO(fpion): dark mode selector, which is a simple dropdown menu with Light, Dark and Auto -->
          </li>
          <!-- TODO(fpion): user avatar with profile and sign-out dropdown -->
        </ul>
      </div>
    </div>
  </nav>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import TarBadge from "@/components/tar/TarBadge.vue";

const environment: string = import.meta.env.MODE;
const { t } = useI18n();
</script>
