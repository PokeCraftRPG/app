<template>
  <main class="container page">
    <h1>{{ title }}</h1>
    <WorldBreadcrumb :current="title" :parent="breadcrumb" />
    <FormProperties class="border-top border-secondary-subtle pt-4" @created="onCreated" @error="handleError" />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, watchEffect } from "vue";
import { useI18n } from "vue-i18n";
import { useRouter } from "vue-router";

import FormProperties from "@/components/pokemonForms/FormProperties.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { Form } from "@/types/pokemonForms";
import { handleErrorKey } from "@/inject";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const router = useRouter();
const { t } = useI18n();

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("forms.title"), to: { name: "Forms" } }));
const title = computed<string>(() => t("forms.create"));

function onCreated(form: Form): void {
  events.push("created");
  router.push({ name: "FormEdit", params: { id: form.id } });
}

watchEffect(() => document.setTitle(title.value));
</script>
