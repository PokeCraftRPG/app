<template>
  <LinkCard :subtitle="subtitle" :title="title" :to="{ name: 'SpeciesDetail', params: { id: species.id } }">
    <div v-if="species.summary" class="card-text mb-2">{{ species.summary }}</div>
    <StatusBlock :actor="species.updatedBy" class="card-text small text-secondary" :date="species.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Species } from "@/types/species";
import { formatSpecies } from "@/utils/format";

const { n, t } = useI18n();

const props = defineProps<{
  species: Species;
}>();

const subtitle = computed<string>(() => t(`species.category.options.${props.species.category}`));
const title = computed<string>(() => formatSpecies(props.species, n));
</script>
