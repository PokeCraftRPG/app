<template>
  <LinkCard :title="region.name ?? region.key" :to="{ name: 'Region', params: { id: region.id } }">
    <div v-if="region.summary" class="card-text">{{ region.summary }}</div>
    <StatusBlock :actor="region.updatedBy" class="card-text mt-2 small text-secondary" :date="region.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Region } from "@/types/regions";

defineProps<{
  region: Region;
}>();
</script>
