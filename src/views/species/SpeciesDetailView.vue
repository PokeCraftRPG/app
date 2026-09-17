<template>
  <main class="container page">
    <div v-if="species">
      <div class="d-flex flex-wrap align-items-center gap-3">
        <h1>{{ title }}</h1>
        <TarBadge class="fs-6" variant="secondary">{{ t(`species.category.options.${species.category}`) }}</TarBadge>
      </div>
      <WorldBreadcrumb :current="title" :parent="breadcrumb" />
      <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
        <strong>{{ t("species.created.lead", { name: title }) }}</strong> {{ t("species.created.help") }}
      </TarAlert>
      <StatusDetail class="mb-3" :subject="species" />
      <form class="border-top border-secondary-subtle pt-4" @submit.prevent="handleSubmit(submit)">
        <KeyAlreadyUsed v-model="keyAlreadyUsed" />
        <div class="row">
          <div class="col-md-6">
            <NameField class="mb-3" v-model="name" />
          </div>
          <div class="col-md-6">
            <KeyField class="mb-3" ref="keyField" required v-model="key" />
          </div>
        </div>
        <div class="row">
          <div class="col-md-4">
            <BaseFriendshipField class="mb-3" v-model="baseFriendship" />
          </div>
          <div class="col-md-4">
            <CatchRateField class="mb-3" v-model="catchRate" />
          </div>
          <div class="col-md-4">
            <GrowthRateField class="mb-3" required v-model="growthRate" />
          </div>
        </div>
        <div class="row">
          <div class="col-md-4">
            <EggCyclesField class="mb-3" v-model="eggCycles" />
          </div>
          <div class="col-md-4">
            <EggGroupField class="mb-3" id="primary-egg-group" label="species.egg.primary" required v-model="primaryEggGroup" />
          </div>
          <div class="col-md-4">
            <EggGroupField
              class="mb-3"
              :disabled="isSecondaryEggDisabled"
              :exclude="secondaryEggExclusions"
              id="secondary-egg-group"
              label="species.egg.secondary"
              v-model="secondaryEggGroup"
            />
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

import BaseFriendshipField from "@/components/species/BaseFriendshipField.vue";
import CatchRateField from "@/components/species/CatchRateField.vue";
import ContentField from "@/components/shared/ContentField.vue";
import EggCyclesField from "@/components/species/EggCyclesField.vue";
import EggGroupField from "@/components/species/EggGroupField.vue";
import GrowthRateField from "@/components/species/GrowthRateField.vue";
import KeyAlreadyUsed from "@/components/shared/KeyAlreadyUsed.vue";
import KeyField from "@/components/shared/KeyField.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import NameField from "@/components/shared/NameField.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarBadge from "@/components/tar/TarBadge.vue";
import TarButton from "@/components/tar/TarButton.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { ApiFailure, ProblemDetails } from "@/types/api";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { EggGroup, GrowthRate, Species, UpdateSpeciesPayload } from "@/types/species";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { handleErrorKey } from "@/inject";
import { readSpecies, updateSpecies } from "@/api/species";
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
const { n, t } = useI18n();

const baseFriendship = ref<number>(0);
const catchRate = ref<number>(1);
const content = ref<string>("");
const eggCycles = ref<number>(1);
const growthRate = ref<GrowthRate | "">("");
const isCreated = ref<boolean>(false);
const isLoading = ref<boolean>(false);
const key = ref<string>("");
const keyAlreadyUsed = ref<boolean>(false);
const keyField = ref<InstanceType<typeof KeyField> | null>(null);
const name = ref<string>("");
const primaryEggGroup = ref<EggGroup | "">("");
const secondaryEggGroup = ref<EggGroup | "">("");
const species = ref<Species>();
const summary = ref<string>("");

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("species.title"), to: { name: "Species" } }));
const isSecondaryEggDisabled = computed<boolean>(() => primaryEggGroup.value === "NoEggsDiscovered" || primaryEggGroup.value === "Ditto");
const secondaryEggExclusions = computed<EggGroup[]>(() => {
  const exclusions: EggGroup[] = ["NoEggsDiscovered", "Ditto"];
  if (primaryEggGroup.value) {
    exclusions.push(primaryEggGroup.value);
  }
  return exclusions;
});
const hasChanges = computed<boolean>(() =>
  Boolean(
    species.value &&
    growthRate.value &&
    primaryEggGroup.value &&
    (species.value.key !== key.value ||
      (species.value.name ?? "") !== name.value ||
      (species.value.summary ?? "") !== summary.value ||
      (species.value.content ?? "") !== content.value ||
      species.value.baseFriendship !== baseFriendship.value ||
      species.value.catchRate !== catchRate.value ||
      species.value.growthRate !== growthRate.value ||
      species.value.eggs.cycles !== eggCycles.value ||
      species.value.eggs.primaryGroup !== primaryEggGroup.value ||
      (species.value.eggs.secondaryGroup ?? "") !== secondaryEggGroup.value),
  ),
);
const title = computed<string>(() => (species.value ? [`#${n(species.value.number, "integer")}`, species.value.name ?? species.value.key].join(" ") : ""));

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && species.value && growthRate.value && primaryEggGroup.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    try {
      const payload: UpdateSpeciesPayload = {
        key: key.value,
        name: { value: name.value },
        summary: { value: summary.value },
        content: { value: content.value },
        baseFriendship: baseFriendship.value,
        catchRate: catchRate.value,
        growthRate: growthRate.value,
        eggs: {
          cycles: eggCycles.value,
          primaryGroup: primaryEggGroup.value,
          secondaryGroup: isSecondaryEggDisabled.value ? null : secondaryEggGroup.value || null,
        },
      };
      species.value = await updateSpecies(species.value.id, payload);
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
      }
      handleError(e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(primaryEggGroup, (value) => {
  if (value === "NoEggsDiscovered" || value === "Ditto" || value === secondaryEggGroup.value) {
    secondaryEggGroup.value = "";
  }
});

watch(
  species,
  (species) => {
    key.value = species?.key ?? "";
    name.value = species?.name ?? "";
    summary.value = species?.summary ?? "";
    content.value = species?.content ?? "";
    baseFriendship.value = species?.baseFriendship ?? 0;
    catchRate.value = species?.catchRate ?? 1;
    growthRate.value = species?.growthRate ?? "";
    eggCycles.value = species?.eggs.cycles ?? 1;
    primaryEggGroup.value = species?.eggs.primaryGroup ?? "";
    secondaryEggGroup.value = species?.eggs.secondaryGroup ?? "";
  },
  { deep: true },
);

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    species.value = await readSpecies(id);
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
