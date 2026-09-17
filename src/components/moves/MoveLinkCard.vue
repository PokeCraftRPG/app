<template>
  <LinkCard :title="formatMove(move)" :to="{ name: 'Move', params: { id: move.id } }">
    <div class="d-flex justify-content-between align-items-center gap-2 mb-2">
      <PokemonTypeImage :type="move.type" height="24" />
      <MoveCategoryBadge :category="move.category" height="24" />
    </div>
    <div v-if="hasMechanics" class="d-flex justify-content-between align-items-center gap-2 mb-2 text-body-secondary">
      <div v-if="move.accuracy">{{ t("moves.accuracy.format", { accuracy: n(move.accuracy, "integer") }) }}</div>
      <div v-if="move.power">{{ t("moves.power.format", { power: n(move.power, "integer") }) }}</div>
      <div v-if="move.powerPoints">{{ t("moves.powerPoints.format", { powerPoints: n(move.powerPoints, "integer") }) }}</div>
    </div>
    <div v-if="move.summary" class="card-text mb-2">{{ move.summary }}</div>
    <StatusBlock :actor="move.updatedBy" class="card-text small text-secondary" :date="move.updatedOn" relative />
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
import { formatMove } from "@/utils/format";

const { n, t } = useI18n();

const props = defineProps<{
  move: Move;
}>();

const hasMechanics = computed<boolean>(() => Boolean(props.move.accuracy || props.move.power || props.move.powerPoints));
</script>
