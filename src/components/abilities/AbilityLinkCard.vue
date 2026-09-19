<template>
  <LinkCard :title="formatAbility(ability)" :to="{ name: 'AbilityEdit', params: { id: ability.id } }">
    <div v-if="ability.summary" class="card-text mb-2">{{ ability.summary }}</div>
    <StatusBlock :actor="ability.updatedBy" class="card-text small text-secondary" :date="ability.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Ability } from "@/types/abilities";
import { formatAbility } from "@/utils/format";

defineProps<{
  ability: Ability;
}>();
</script>
