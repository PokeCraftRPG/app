<template>
  <main class="container page">
    <h1>{{ title }}</h1>
    <WorldBreadcrumb :current="title" :parent="breadcrumb" />
    <AbilityProperties @created="onCreated" @error="handleError" />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, watchEffect } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import AbilityProperties from "@/components/abilities/AbilityProperties.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Ability } from "@/types/abilities";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import { handleErrorKey } from "@/inject";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const router = useRouter();
const { t } = useI18n();

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("abilities.title"), to: { name: "Abilities" } }));
const title = computed<string>(() => t("abilities.create"));

function onCreated(ability: Ability): void {
  events.push("created");
  router.push({ name: "AbilityEdit", params: { id: ability.id } });
}

watchEffect(() => document.setTitle(title.value));
</script>
