<template>
  <LinkCard :title="ability.name ?? ability.key" :to="{ name: 'AbilityEdit', params: { id: ability.id } }">
    <div v-if="ability.summary" class="card-text">{{ ability.summary }}</div>
    <StatusBlock :actor="ability.updatedBy" class="card-text mt-2 small text-secondary" :date="ability.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Ability } from "@/types/abilities";

defineProps<{
  ability: Ability;
}>();
</script>
