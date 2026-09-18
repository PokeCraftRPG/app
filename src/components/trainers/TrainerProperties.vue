<template>
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
        <MemberField class="mb-3" :members="members" :model-value="member?.id" @selected="member = $event" />
      </div>
      <div class="col-lg-6">
        <PartyLimitField class="mb-3" v-model="partyLimit" />
      </div>
    </div>
    <SummaryField class="mb-3" v-model="summary" />
    <ContentField class="mb-3" v-model="content" />
    <div class="d-flex justify-content-end mb-3">
      <SaveButton
        :disabled="!hasChanges || isLoading"
        :icon="trainer ? 'fas fa-floppy-disk' : 'fas fa-plus'"
        :loading="isLoading"
        :text="trainer ? 'actions.save' : 'actions.create'"
      />
    </div>
  </form>
</template>

<script setup lang="ts">
import { computed, nextTick, onMounted, ref, watch } from "vue";
import { stringUtils } from "logitar-js";

import ContentField from "@/components/shared/ContentField.vue";
import GenderField from "./GenderField.vue";
import KeyAlreadyUsed from "@/components/shared/KeyAlreadyUsed.vue";
import KeyField from "@/components/shared/KeyField.vue";
import LicenseField from "./LicenseField.vue";
import MemberField from "@/components/membership/MemberField.vue";
import NameField from "@/components/shared/NameField.vue";
import PartyLimitField from "./PartyLimitField.vue";
import SaveButton from "@/components/shared/SaveButton.vue";
import SummaryField from "@/components/shared/SummaryField.vue";
import type { Actor, ApiFailure, ProblemDetails } from "@/types/api";
import type { CreateOrReplaceTrainerPayload, Gender, Trainer, TrainerFilters, UpdateTrainerPayload } from "@/types/trainers";
import { ErrorCodes, StatusCodes } from "@/types/api";
import { createTrainer, getTrainerFilters, updateTrainer } from "@/api/trainers";
import { useForm } from "@/forms";

const { slugify } = stringUtils;

const props = defineProps<{
  trainer?: Trainer;
}>();

const emit = defineEmits<{
  (e: "created", value: Trainer): void;
  (e: "error", value: unknown): void;
  (e: "updated", value: Trainer): void;
}>();

const content = ref<string>("");
const gender = ref<Gender | "">("");
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

const hasChanges = computed<boolean>(() => {
  const trainer: Trainer | undefined = props.trainer;
  return (
    key.value !== (trainer?.key ?? "") ||
    name.value !== (trainer?.name ?? "") ||
    summary.value !== (trainer?.summary ?? "") ||
    content.value !== (trainer?.content ?? "") ||
    license.value !== (trainer?.license ?? "") ||
    gender.value !== (trainer?.gender ?? "") ||
    (member.value?.id ?? "") !== (trainer?.member?.id ?? "") ||
    partyLimit.value !== (trainer?.partyLimit ?? 0)
  );
});

const { handleSubmit, reinitialize } = useForm();
async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    keyAlreadyUsed.value = false;
    licenseAlreadyUsed.value = false;
    try {
      if (props.trainer) {
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
        const updated: Trainer = await updateTrainer(props.trainer.id, payload);
        emit("updated", updated);
      } else {
        const payload: CreateOrReplaceTrainerPayload = {
          key: key.value,
          name: name.value,
          summary: summary.value,
          content: content.value,
          license: license.value || null,
          gender: gender.value || null,
          money: 0,
          partyLimit: partyLimit.value || null,
          memberId: member.value?.id ?? null,
        };
        const created: Trainer = await createTrainer(payload);
        emit("created", created);
      }
      nextTick(reinitialize);
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
      emit("error", e);
    } finally {
      isLoading.value = false;
    }
  }
}

watch(name, (newValue, oldValue) => {
  if (!key.value || key.value === slugify(oldValue)) {
    key.value = slugify(newValue);
  }
});
watch(
  () => props.trainer,
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
  { deep: true, immediate: true },
);

onMounted(async () => {
  try {
    const filters: TrainerFilters = await getTrainerFilters();
    members.value = [...filters.members];
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>
