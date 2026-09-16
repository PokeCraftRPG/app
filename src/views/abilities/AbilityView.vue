<template>
  <main class="container page">
    <div v-if="ability">
      <h1>{{ title }}</h1>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("abilities.created.lead", { name: title }) }}</strong> {{ t("abilities.created.help") }}
      </TarAlert>
      <StatusDetail class="mb-3" :subject="ability" />
      <form class="border-top border-secondary-subtle pt-4" @submit.prevent="handleSubmit(submit)">
        <KeyAlreadyUsed v-model="keyAlreadyUsed" />
        <div class="row">
          <div class="col-lg-6">
            <NameField class="mb-3" v-model="name" />
          </div>
          <div class="col-lg-6">
            <KeyField class="mb-3" ref="keyField" required v-model="key" />
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
import KeyField from "@/components/shared/KeyField.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import NameField from "@/components/shared/NameField.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarButton from "@/components/tar/TarButton.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Ability, CreateOrReplaceAbilityPayload } from "@/types/abilities";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import { ErrorCodes, StatusCodes, type ApiFailure, type ProblemDetails } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readAbility, replaceAbility } from "@/api/abilities";
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

const ability = ref<Ability>();
const content = ref<string>("");
const isCreated = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const key = ref<string>("");
const keyAlreadyUsed = ref<boolean>(false);
const keyField = ref<InstanceType<typeof KeyField> | null>(null);
const name = ref<string>("");
const summary = ref<string>("");

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("abilities.title"), to: { name: "Abilities" } }));
const hasChanges = computed<boolean>(() =>
  Boolean(
    ability.value &&
    (ability.value.key !== key.value ||
      (ability.value.name ?? "") !== name.value ||
      (ability.value.summary ?? "") !== summary.value ||
      (ability.value.content ?? "") !== content.value),
  ),
);
const title = computed<string>(() => ability.value?.name ?? ability.value?.key ?? "");

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && ability.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    try {
      const payload: CreateOrReplaceAbilityPayload = {
        key: key.value,
        name: name.value || null,
        summary: summary.value || null,
        content: content.value,
      };
      ability.value = await replaceAbility(ability.value.id, payload);
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
  ability,
  (ability) => {
    key.value = ability?.key ?? "";
    name.value = ability?.name ?? "";
    summary.value = ability?.summary ?? "";
    content.value = ability?.content ?? "";
  },
  { deep: true },
);

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    ability.value = await readAbility(id);
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
