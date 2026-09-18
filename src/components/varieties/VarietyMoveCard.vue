<template>
  <div class="position-relative h-100">
    <TarCard class="clickable h-100" @click="$emit('click')">
      <h5 class="card-title">{{ formatMove(varietyMove.move) }}</h5>
      <h6 class="card-subtitle mb-2 text-body-secondary">{{ subtitle }}</h6>
      <div class="d-flex justify-content-between align-items-center gap-2">
        <PokemonTypeImage :type="varietyMove.move.type" height="24" />
        <MoveCategoryBadge :category="varietyMove.move.category" height="24" />
      </div>
    </TarCard>
    <div class="position-absolute top-0 end-0 m-2" @click.stop>
      <button type="button" class="btn btn-sm" @click="$emit('remove')">
        <font-awesome-icon icon="fas fa-xmark" aria-hidden="true" />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import MoveCategoryBadge from "@/components/moves/MoveCategoryBadge.vue";
import PokemonTypeImage from "@/components/pokemon/PokemonTypeImage.vue";
import TarCard from "@/components/tar/TarCard.vue";
import type { VarietyMove } from "@/types/varieties";
import { formatMove } from "@/utils/format";

const { n, t } = useI18n();

const props = defineProps<{
  varietyMove: VarietyMove;
}>();

defineEmits<{
  (e: "click"): void;
  (e: "remove"): void;
}>();

const subtitle = computed<string>(() => {
  const method: string = t(`varieties.learningMethod.options.${props.varietyMove.learningMethod}`);
  return props.varietyMove.level ? [method, t("varieties.level.format", { level: n(props.varietyMove.level, "integer") })].join(" · ") : method;
});
</script>
