<template>
  <SelectField
    :disabled="!options.length"
    :id="id"
    :label="label ? t(label) : undefined"
    :model-value="modelValue"
    :options="options"
    :placeholder="placeholder ? t(placeholder) : undefined"
    @update:model-value="onModelValueUpdate($event ?? '')"
  >
    <template v-if="member" #append>
      <span class="input-group-text">
        <TarAvatar :display-name="member.displayName" :email-address="member.emailAddress ?? ''" :url="member.pictureUrl ?? ''" />
      </span>
    </template>
  </SelectField>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import SelectField from "@/components/forms/SelectField.vue";
import TarAvatar from "@/components/tar/TarAvatar.vue";
import type { Actor } from "@/types/api";
import type { SelectOption } from "@/types/tar/select";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    members?: Actor[];
    modelValue?: string;
    placeholder?: string;
  }>(),
  {
    id: "member",
    label: "members.label",
    members: () => [],
    placeholder: "members.placeholder",
  },
);

const emit = defineEmits<{
  (e: "selected", value: Actor | undefined): void;
  (e: "update:model-value", value: string): void;
}>();

const member = computed<Actor | undefined>(() => props.members.find((member) => member.id === props.modelValue));
const options = computed<SelectOption[]>(() => props.members.map((actor) => ({ text: actor.displayName, value: actor.id })));

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);

  const member: Actor | undefined = props.members.find((member) => member.id === id);
  emit("selected", member);
}
</script>
