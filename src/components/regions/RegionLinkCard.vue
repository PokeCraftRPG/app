<template>
  <LinkCard :title="formatRegion(region)" :to="{ name: 'RegionEdit', params: { id: region.id } }">
    <div v-if="region.summary" class="card-text mb-2">{{ region.summary }}</div>
    <StatusBlock :actor="region.updatedBy" class="card-text small text-secondary" :date="region.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Region } from "@/types/regions";
import { formatRegion } from "@/utils/format";

defineProps<{
  region: Region;
}>();
</script>
