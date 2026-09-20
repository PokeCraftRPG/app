<template>
  <LinkCard :subtitle="subtitle" :title="title" :to="{ name: 'FormEdit', params: { id: form.id } }">
    <div class="d-flex justify-content-between align-items-center gap-2 mb-2">
      <PokemonTypeImage :type="form.types.primary" height="24" />
      <PokemonTypeImage v-if="form.types.secondary" :type="form.types.secondary" height="24" />
    </div>
    <div class="d-flex justify-content-between align-items-center gap-2 mb-2 text-body-secondary">
      <div>{{ formatAbility(form.abilities.primary) }}</div>
      <div v-if="form.abilities.secondary">{{ formatAbility(form.abilities.secondary) }}</div>
      <div v-if="form.abilities.hidden"><font-awesome-icon icon="fas fa-eye-slash" aria-hidden="true" />&nbsp;{{ formatAbility(form.abilities.hidden) }}</div>
    </div>
    <div v-if="form.summary" class="card-text mb-2">{{ form.summary }}</div>
    <StatusBlock :actor="form.updatedBy" class="card-text small text-secondary" :date="form.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import LinkCard from "@/components/shared/LinkCard.vue";
import PokemonTypeImage from "@/components/pokemon/PokemonTypeImage.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Form } from "@/types/pokemonForms";
import { formatAbility, formatForm } from "@/utils/format";

const { t } = useI18n();

const props = defineProps<{
  form: Form;
}>();

const subtitle = computed<string>(() => t(`forms.category.options.${props.form.category}`));
const title = computed<string>(() => formatForm(props.form));
</script>
