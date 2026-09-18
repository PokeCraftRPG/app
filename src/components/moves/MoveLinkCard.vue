<template>
  <LinkCard :title="move.name ?? move.key" :to="{ name: 'MoveEdit', params: { id: move.id } }">
    <div class="d-flex justify-content-between align-items-center gap-2 mb-2">
      <PokemonTypeImage :type="move.type" height="24" />
      <MoveCategoryBadge :category="move.category" height="24" />
    </div>
    <div v-if="hasMechanics" class="d-flex justify-content-between align-items-center gap-2 mb-2 text-body-secondary">
      <div>{{ t("moves.accuracy.format", { accuracy: move.accuracy ? n(move.accuracy, "integer") : "—" }) }}</div>
      <div>{{ t("moves.power.format", { power: move.power ? n(move.power, "integer") : "—" }) }}</div>
      <div>{{ t("moves.powerPoints.format", { powerPoints: move.powerPoints ? n(move.powerPoints, "integer") : "—" }) }}</div>
    </div>
    <div v-if="move.summary" class="card-text">{{ move.summary }}</div>
    <StatusBlock :actor="move.updatedBy" class="card-text mt-2 small text-secondary" :date="move.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import LinkCard from "@/components/shared/LinkCard.vue";
import MoveCategoryBadge from "@/components/moves/MoveCategoryBadge.vue";
import PokemonTypeImage from "@/components/pokemon/PokemonTypeImage.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Move } from "@/types/moves";

const { n, t } = useI18n();

const props = defineProps<{
  move: Move;
}>();

const hasMechanics = computed<boolean>(() => Boolean(props.move.accuracy || props.move.power || props.move.powerPoints));
</script>
