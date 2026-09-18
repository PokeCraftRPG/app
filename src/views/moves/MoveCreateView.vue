<template>
  <main class="container page">
    <h1>{{ title }}</h1>
    <WorldBreadcrumb :current="title" :parent="breadcrumb" />
    <MoveProperties @created="onCreated" @error="handleError" />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, watchEffect } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import MoveProperties from "@/components/moves/MoveProperties.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { Move } from "@/types/moves";
import { handleErrorKey } from "@/inject";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const router = useRouter();
const { t } = useI18n();

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("moves.title"), to: { name: "Moves" } }));
const title = computed<string>(() => t("moves.create"));

function onCreated(move: Move): void {
  events.push("created");
  router.push({ name: "MoveEdit", params: { id: move.id } });
}

watchEffect(() => document.setTitle(title.value));
</script>
