<template>
  <main class="container page">
    <h1>{{ title }}</h1>
    <WorldBreadcrumb :current="title" :parent="breadcrumb" />
    <TrainerProperties @created="onCreated" @error="handleError" />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, watchEffect } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import TrainerProperties from "@/components/trainers/TrainerProperties.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { Trainer } from "@/types/trainers";
import { handleErrorKey } from "@/inject";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const router = useRouter();
const { t } = useI18n();

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("trainers.title"), to: { name: "Trainers" } }));
const title = computed<string>(() => t("trainers.create"));

function onCreated(trainer: Trainer): void {
  events.push("created");
  router.push({ name: "TrainerEdit", params: { id: trainer.id } });
}

watchEffect(() => document.setTitle(title.value));
</script>
