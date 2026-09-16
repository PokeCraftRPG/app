<template>
  <main class="container page">
    <div v-if="region">
      <h1>{{ title }}</h1>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("regions.created.lead", { name: title }) }}</strong> {{ t("regions.created.help") }}
      </TarAlert>
      <StatusDetail class="mb-3" :subject="region" />
      <form class="border-top border-secondary-subtle pt-4" @submit.prevent="handleSubmit(submit)">
        <KeyAlreadyUsed v-model="keyAlreadyUsed" help="regions.key.alreadyUsed.help" lead="regions.key.alreadyUsed.lead" />
        <div class="row">
          <div class="col-lg-6">
            <NameField class="mb-3" v-model="name" />
          </div>
          <div class="col-lg-6">
            <KeyField class="mb-3" label="regions.key.label" ref="keyField" required v-model="key" />
          </div>
        </div>
        <SummaryField class="mb-3" v-model="summary" />
        <ContentField class="mb-3" v-model="content" />
        <div class="d-flex justify-content-end mb-3">
          <TarButton
            :disabled="!hasChanges || isLoading"
            icon="fas fa-floppy-disk"
            :loading="isLoading"
            size="large"
            :status="t('loading')"
            :text="t('actions.save')"
            type="submit"
          />
        </div>
      </form>
    </div>
    <LoadingSpinner v-else />
  </main>
</template>

<script setup lang="ts">
import { computed, inject, onMounted, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import ContentField from "@/components/shared/ContentField.vue";
import KeyAlreadyUsed from "@/components/shared/KeyAlreadyUsed.vue";
import KeyField from "@/components/worlds/KeyField.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import NameField from "@/components/shared/NameField.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarButton from "@/components/tar/TarButton.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { CreateOrReplaceRegionPayload, Region } from "@/types/regions";
import { ErrorCodes, StatusCodes, type ApiFailure, type ProblemDetails } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readRegion, replaceRegion } from "@/api/regions";
import { useDocument } from "@/composables/document";
import { useEventStore } from "@/stores/event";
import { useForm } from "@/forms";
import { useToastStore } from "@/stores/toast";

const document = useDocument();
const events = useEventStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const content = ref<string>("");
const isCreated = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const key = ref<string>("");
const keyAlreadyUsed = ref<boolean>(false);
const keyField = ref<InstanceType<typeof KeyField> | null>(null);
const name = ref<string>("");
const region = ref<Region>();
const summary = ref<string>("");

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("regions.title"), to: { name: "Regions" } }));
const hasChanges = computed<boolean>(() =>
  Boolean(
    region.value &&
    (region.value.key !== key.value ||
      (region.value.name ?? "") !== name.value ||
      (region.value.summary ?? "") !== summary.value ||
      (region.value.content ?? "") !== content.value),
  ),
);
const title = computed<string>(() => region.value?.name ?? region.value?.key ?? "");

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && region.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    try {
      const payload: CreateOrReplaceRegionPayload = {
        key: key.value,
        name: name.value || null,
        summary: summary.value || null,
        content: content.value,
      };
      region.value = await replaceRegion(region.value.id, payload);
      isCreated.value = false;
      reinitialize();
      toasts.success("saved");
    } catch (e: unknown) {
      const failure = e as ApiFailure;
      if (failure.status === StatusCodes.Conflict) {
        const problemDetails = failure.data as ProblemDetails;
        if (problemDetails.error && problemDetails.error.code === ErrorCodes.KeyAlreadyUsed) {
          keyAlreadyUsed.value = true;
          keyField.value?.focus();
          return;
        }
      }
      handleError(e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(
  region,
  (region) => {
    key.value = region?.key ?? "";
    name.value = region?.name ?? "";
    summary.value = region?.summary ?? "";
    content.value = region?.content ?? "";
  },
  { deep: true },
);

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    region.value = await readRegion(id);
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
