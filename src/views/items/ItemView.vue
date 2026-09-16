<template>
  <main class="container page">
    <div v-if="item">
      <div class="d-flex justify-content-between align-items-start gap-4">
        <div class="flex-grow-1">
          <div class="d-flex flex-wrap align-items-center gap-3">
            <h1>{{ title }}</h1>
            <TarBadge class="fs-6" variant="secondary">{{ t(`items.category.options.${item.category}`) }}</TarBadge>
          </div>
          <WorldBreadcrumb :current="title" :parent="breadcrumb" />
          <TarAlert :close="t('actions.close')" dismissible variant="success" v-model="isCreated">
            <strong>{{ t("items.created.lead", { name: title }) }}</strong> {{ t("items.created.help") }}
          </TarAlert>
          <StatusDetail class="mb-3" :subject="item" />
        </div>
        <ImageAsset v-if="item.sprite" :alt="t('sprite.alt', { name: title })" :asset="item.sprite" height="144" />
      </div>
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
        <SummaryField class="mb-3" v-model="summary" />
        <ContentField class="mb-3" v-model="content" />
        <div class="row">
          <div class="col-md-6">
            <PriceField class="mb-3" v-model="price" />
          </div>
          <div class="col-md-6">
            <WeightField class="mb-3" v-model="weight" />
          </div>
        </div>
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
import ImageAsset from "@/components/shared/ImageAsset.vue";
import KeyAlreadyUsed from "@/components/shared/KeyAlreadyUsed.vue";
import KeyField from "@/components/shared/KeyField.vue";
import LoadingSpinner from "@/components/shared/LoadingSpinner.vue";
import NameField from "@/components/shared/NameField.vue";
import PriceField from "@/components/items/PriceField.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import TarAlert from "@/components/tar/TarAlert.vue";
import TarBadge from "@/components/tar/TarBadge.vue";
import TarButton from "@/components/tar/TarButton.vue";
import WeightField from "@/components/items/WeightField.vue";
import WorldBreadcrumb from "@/components/shared/WorldBreadcrumb.vue";
import type { ApiFailure, ProblemDetails } from "@/types/api";
import type { Breadcrumb } from "@/types/tar/breadcrumb";
import type { CreateOrReplaceItemPayload, Item } from "@/types/items";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { fromHundredths, toHundredths } from "@/utils/number";
import { handleErrorKey } from "@/inject";
import { readItem, replaceItem } from "@/api/items";
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
const item = ref<Item>();
const key = ref<string>("");
const keyAlreadyUsed = ref<boolean>(false);
const keyField = ref<InstanceType<typeof KeyField> | null>(null);
const name = ref<string>("");
const price = ref<number>(0);
const summary = ref<string>("");
const weight = ref<number>(0);

const breadcrumb = computed<Breadcrumb>(() => ({ text: t("items.title"), to: { name: "Items" } }));
const hasChanges = computed<boolean>(() =>
  Boolean(
    item.value &&
    (item.value.key !== key.value ||
      (item.value.name ?? "") !== name.value ||
      (item.value.summary ?? "") !== summary.value ||
      (item.value.content ?? "") !== content.value ||
      (fromHundredths(item.value.price) ?? 0) !== price.value ||
      (fromHundredths(item.value.weight) ?? 0) !== weight.value),
  ),
);
const title = computed<string>(() => item.value?.name ?? item.value?.key ?? "");

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value && item.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    try {
      const payload: CreateOrReplaceItemPayload = {
        category: item.value.category,
        key: key.value,
        name: name.value,
        summary: summary.value,
        content: content.value,
        price: toHundredths(price.value) || undefined,
        weight: toHundredths(price.value) || undefined,
        spriteId: item.value.sprite?.id,
      };
      item.value = await replaceItem(item.value.id, payload);
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
  item,
  (item) => {
    key.value = item?.key ?? "";
    name.value = item?.name ?? "";
    summary.value = item?.summary ?? "";
    content.value = item?.content ?? "";
    price.value = fromHundredths(item?.price) ?? 0;
    weight.value = fromHundredths(item?.weight) ?? 0;
  },
  { deep: true },
);

onMounted(async () => {
  try {
    const id: string = (Array.isArray(route.params.id) ? route.params.id[0] : route.params.id) ?? "";
    item.value = await readItem(id);
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
