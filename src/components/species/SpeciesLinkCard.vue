<template>
  <LinkCard :subtitle="category" :title="title" :to="{ name: 'SpeciesDetail', params: { id: species.id } }">
    <div v-if="species.summary" class="card-text">{{ species.summary }}</div>
    <StatusBlock :actor="species.updatedBy" class="card-text mt-2 small text-secondary" :date="species.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Species } from "@/types/species";

const { n, t } = useI18n();

const props = defineProps<{
  species: Species;
}>();

const category = computed<string>(() => t(`species.category.options.${props.species.category}`));
const title = computed<string>(() => `#${n(props.species.number, "pokemonNumber")} ${props.species.name ?? props.species.key}`);
</script>
