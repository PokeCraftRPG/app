<template>
  <main class="container page">
    <div v-if="trainer">
      <div class="d-flex justify-content-between align-items-start gap-4">
        <div class="flex-grow-1">
          <h1>{{ title }}</h1>
          <WorldBreadcrumb :current="title" :parent="breadcrumb" />
          <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
            <strong>{{ t("trainers.created.lead", { name: title }) }}</strong> {{ t("trainers.created.help") }}
          </TarAlert>
          <StatusDetail class="mb-3" :subject="trainer" />
        </div>
        <ImageAsset v-if="trainer.sprite" :alt="t('sprite.alt', { name: title })" :asset="trainer.sprite" height="144" />
      </div>
      <form class="border-top border-secondary-subtle pt-4" @submit.prevent="handleSubmit(submit)">
        <KeyAlreadyUsed v-model="keyAlreadyUsed" />
        <KeyAlreadyUsed v-model="licenseAlreadyUsed" help="trainers.license.alreadyUsed.help" lead="trainers.license.alreadyUsed.lead" />
        <div class="row">
          <div class="col-lg-6">
            <NameField class="mb-3" v-model="name" />
          </div>
          <div class="col-lg-6">
            <KeyField class="mb-3" ref="keyField" required v-model="key" />
          </div>
        </div>
        <div class="row">
          <div class="col-lg-6">
            <GenderField class="mb-3" v-model="gender" />
          </div>
          <div class="col-lg-6">
            <LicenseField class="mb-3" ref="licenseField" v-model="license" />
          </div>
        </div>
        <div class="row">
          <div class="col-lg-6">
            <MemberField :members="members" :model-value="member?.id" @selected="member = $event" />
          </div>
          <div class="col-lg-6">
            <PartyLimitField class="mb-3" v-model="partyLimit" />
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
import GenderField from "@/components/trainers/GenderField.vue";
import ImageAsset from "@/components/shared/ImageAsset.vue";
import KeyAlreadyUsed from "@/components/shared/KeyAlreadyUsed.vue";
import KeyField from "@/components/shared/KeyField.vue";
import LicenseField from "@/components/trainers/LicenseField.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import MemberField from "@/components/membership/MemberField.vue";
import NameField from "@/components/shared/NameField.vue";
import PartyLimitField from "@/components/trainers/PartyLimitField.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarButton from "@/components/tar/TarButton.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { Actor, ApiFailure, ProblemDetails } from "@/types/api";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { Gender, Trainer, TrainerFilters, UpdateTrainerPayload } from "@/types/trainers";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { getTrainerFilters, readTrainer, updateTrainer } from "@/api/trainers";
import { handleErrorKey } from "@/inject";
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
const gender = ref<Gender | "">("");
const isCreated = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const key = ref<string>("");
const keyAlreadyUsed = ref<boolean>(false);
const keyField = ref<InstanceType<typeof KeyField> | null>(null);
const license = ref<string>("");
const licenseAlreadyUsed = ref<boolean>(false);
const licenseField = ref<InstanceType<typeof LicenseField> | null>(null);
const member = ref<Actor>();
const members = ref<Actor[]>([]);
const name = ref<string>("");
const partyLimit = ref<number>(0);
const summary = ref<string>("");
const trainer = ref<Trainer>();

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("trainers.title"), to: { name: "Trainers" } }));
const hasChanges = computed<boolean>(() =>
  Boolean(
    trainer.value &&
    (trainer.value.key !== key.value ||
      (trainer.value.name ?? "") !== name.value ||
      (trainer.value.summary ?? "") !== summary.value ||
      (trainer.value.content ?? "") !== content.value ||
      (trainer.value.license ?? "") !== license.value ||
      (trainer.value.gender ?? "") !== gender.value ||
      (trainer.value.member?.id ?? "") !== (member.value?.id ?? "") ||
      (trainer.value.partyLimit ?? 0) !== partyLimit.value),
  ),
);
const title = computed<string>(() => trainer.value?.name ?? trainer.value?.key ?? "");

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && trainer.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    licenseAlreadyUsed.value = false;
    try {
      const payload: UpdateTrainerPayload = {
        key: key.value,
        name: { value: name.value },
        summary: { value: summary.value },
        content: { value: content.value },
        license: { value: license.value },
        gender: { value: gender.value || null },
        partyLimit: { value: partyLimit.value || null },
        memberId: { value: member.value?.id ?? null },
      };
      trainer.value = await updateTrainer(trainer.value.id, payload);
      isCreated.value = false;
      reinitialize();
      toasts.success("saved");
    } catch (e: unknown) {
      const failure = e as ApiFailure;
      if (failure.status === StatusCodes.Conflict) {
        const problemDetails = failure.data as ProblemDetails;
        if (problemDetails.error?.code === ErrorCodes.KeyAlreadyUsed) {
          keyAlreadyUsed.value = true;
          keyField.value?.focus();
          return;
        }
        if (problemDetails.error?.code === ErrorCodes.LicenseAlreadyUsed) {
          licenseAlreadyUsed.value = true;
          licenseField.value?.focus();
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
  trainer,
  (trainer) => {
    key.value = trainer?.key ?? "";
    name.value = trainer?.name ?? "";
    summary.value = trainer?.summary ?? "";
    content.value = trainer?.content ?? "";
    license.value = trainer?.license ?? "";
    gender.value = trainer?.gender ?? "";
    member.value = trainer?.member ? { ...trainer.member } : undefined;
    partyLimit.value = trainer?.partyLimit ?? 0;
  },
  { deep: true },
);

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    trainer.value = await readTrainer(id);
    isCreated.value = events.shift() === "created";
    document.setTitle(title.value);

    const filters: TrainerFilters = await getTrainerFilters();
    members.value = [...filters.members];
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
