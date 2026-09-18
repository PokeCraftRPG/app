<template>
  <main class="container page">
    <h1>{{ title }}</h1>
    <WorldBreadcrumb :current="title" :parent="breadcrumb" />
    <RegionProperties @created="onCreated" @error="handleError" />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, watchEffect } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import RegionProperties from "@/components/regions/RegionProperties.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { Region } from "@/types/regions";
import { handleErrorKey } from "@/inject";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const router = useRouter();
const { t } = useI18n();

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("regions.title"), to: { name: "Regions" } }));
const title = computed<string>(() => t("regions.create"));

function onCreated(region: Region): void {
  events.push("created");
  router.push({ name: "RegionEdit", params: { id: region.id } });
}

watchEffect(() => document.setTitle(title.value));
</script>
