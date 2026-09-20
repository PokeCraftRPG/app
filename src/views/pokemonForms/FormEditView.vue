<template>
  <main class="container page">
    <div v-if="form">
      <h1>{{ title }}</h1>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("forms.created.lead", { name: title }) }}</strong> {{ t("forms.created.help") }}
      </TarAlert>
      <StatusDetail class="mb-3" :subject="form" />
      <FormProperties class="border-top border-secondary-subtle pt-4" :form="form" @error="handleError" @updated="onUpdated" />
    </div>
    <LoadingSpinner v-else />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import FormProperties from "@/components/pokemonForms/FormProperties.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { ApiFailure } from "@/types/api";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { Form } from "@/types/pokemonForms";
import { StatusCodes } from "@/types/api";
import { formatForm } from "@/utils/format";
import { handleErrorKey } from "@/inject";
import { readForm } from "@/api/pokemonForms";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";
import { useToastStore } from "@/stores/toast";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const form = ref<Form>();
const isCreated = ref<boolean>(false);

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("forms.title"), to: { name: "Forms" } }));
const title = computed<string>(() => (form.value ? formatForm(form.value) : ""));

function onUpdated(updated: Form): void {
  form.value = updated;
  isCreated.value = false;
  document.setTitle(title.value);
  toasts.success("saved");
}

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    form.value = await readForm(id);
    isCreated.value = events.shift() === "created";
    document.setTitle(title.value);
  } catch (e: unknown) {
    const failure = e as ApiFailure;
    if (failure.status === StatusCodes.NotFound) {
      router.push("/not-found");
    } else {
      handleError(e);
    }
  }
});
</script>
