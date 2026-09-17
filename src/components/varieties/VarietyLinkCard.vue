<template>
  <LinkCard :subtitle="subtitle" :title="title" :to="{ name: 'VarietyDetail', params: { id: variety.id } }">
    <DefaultBadge v-if="variety.isDefault" class="mb-2" />
    <div v-if="variety.summary" class="card-text mb-2">{{ variety.summary }}</div>
    <StatusBlock :actor="variety.updatedBy" class="card-text small text-secondary" :date="variety.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import DefaultBadge from "./DefaultBadge.vue";
import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Variety } from "@/types/varieties";
import { formatSpecies, formatVariety } from "@/utils/format";

const { n } = useI18n();

const props = defineProps<{
  variety: Variety;
}>();

const subtitle = computed<string>(() => formatSpecies(props.variety.species, n));
const title = computed<string>(() => formatVariety(props.variety));
</script>
