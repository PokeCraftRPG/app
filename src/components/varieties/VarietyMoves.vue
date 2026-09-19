<template>
  <div>
    <div class="d-flex justify-content-end mb-3">
      <TarButton :disabled="!moves.length" icon="fas fa-plus" size="large" :text="t('actions.add')" @click="add" />
    </div>
    <section v-if="varietyMoves.length">
      <div class="row">
        <div v-for="entry in varietyMoves" :key="entry.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
          <VarietyMoveCard :variety-move="entry" @click="edit(entry)" @remove="remove(entry)" />
        </div>
      </div>
    </section>
    <section v-else class="text-center text-body-secondary py-4">
      <p class="mb-0">{{ t("varieties.moves.empty") }}</p>
    </section>
    <EditVarietyMove ref="editModal" :moves="moves" :variety-id="variety.id" @error="$emit('error', $event)" @saved="updated" />
    <RemoveVarietyMove ref="removeModal" :variety-id="variety.id" @error="$emit('error', $event)" @removed="updated" />
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import EditVarietyMove from "./EditVarietyMove.vue";
import RemoveVarietyMove from "./RemoveVarietyMove.vue";
import VarietyMoveCard from "./VarietyMoveCard.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { MoveSummary } from "@/types/moves";
import type { Variety, VarietyFilters, VarietyMove } from "@/types/varieties";
import { formatMove } from "@/utils/format";
import { getVarietyFilters } from "@/api/varieties";

const { t } = useI18n();

const props = defineProps<{
  variety: Variety;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Variety): void;
}>();

const editModal = ref<InstanceType<typeof EditVarietyMove> | null>(null);
const moves = ref<MoveSummary[]>([]);
const removeModal = ref<InstanceType<typeof RemoveVarietyMove> | null>(null);

const varietyMoves = computed<VarietyMove[]>(() =>
  [...props.variety.moves].sort((a, b) => {
    const byMethod: number = a.learningMethod.localeCompare(b.learningMethod);
    if (byMethod !== 0) {
      return byMethod;
    }
    const byLevel: number = (a.level ?? 0) - (b.level ?? 0);
    if (byLevel !== 0) {
      return byLevel;
    }
    return formatMove(a.move).localeCompare(formatMove(b.move));
  }),
);

function add(): void {
  editModal.value?.open();
}
function edit(entry: VarietyMove): void {
  editModal.value?.open(entry);
}
function remove(entry: VarietyMove): void {
  removeModal.value?.open(entry);
}

function updated(variety: Variety): void {
  emit("updated", variety);
}

onMounted(async () => {
  try {
    const filters: VarietyFilters = await getVarietyFilters();
    moves.value = filters.moves;
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>
