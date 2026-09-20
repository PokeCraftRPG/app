<template>
  <LinkCard :subtitle="subtitle" :title="title" :to="{ name: 'FormEdit', params: { id: form.id } }">
    <div class="card-text mb-2">{{ category }}</div>
    <div v-if="form.summary" class="card-text mb-2">{{ form.summary }}</div>
    <StatusBlock :actor="form.updatedBy" class="card-text small text-secondary" :date="form.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Form } from "@/types/pokemonForms";
import { formatForm, formatVariety } from "@/utils/format";

const { t } = useI18n();

const props = defineProps<{
  form: Form;
}>();

const category = computed<string>(() => t(`forms.category.options.${props.form.category}`));
const subtitle = computed<string>(() => formatVariety(props.form.variety));
const title = computed<string>(() => formatForm(props.form));
</script>
