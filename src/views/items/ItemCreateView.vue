<template>
  <main class="container page">
    <h1>{{ title }}</h1>
    <WorldBreadcrumb :current="title" :parent="breadcrumb" />
    <ItemProperties @created="onCreated" @error="handleError" />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, watchEffect } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import ItemProperties from "@/components/items/ItemProperties.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { Item } from "@/types/items";
import { handleErrorKey } from "@/inject";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const router = useRouter();
const { t } = useI18n();

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("items.title"), to: { name: "Items" } }));
const title = computed<string>(() => t("items.create"));

function onCreated(item: Item): void {
  events.push("created");
  router.push({ name: "ItemEdit", params: { id: item.id } });
}

watchEffect(() => document.setTitle(title.value));
</script>
