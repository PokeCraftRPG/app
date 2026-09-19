<template>
  <div>
    <div class="d-flex justify-content-end mb-3">
      <TarButton :disabled="!availableRegions.length" icon="fas fa-plus" size="large" :text="t('actions.add')" @click="add" />
    </div>
    <section v-if="regionalNumbers.length">
      <div class="row">
        <div v-for="entry in regionalNumbers" :key="entry.region.id" class="col-md-6 col-lg-4 col-xl-3 mb-3">
          <RegionalNumberCard :regional-number="entry" @click="edit(entry)" @remove="remove(entry)" />
        </div>
      </div>
    </section>
    <section v-else class="text-center text-body-secondary py-4">
      <p class="mb-0">{{ t("species.regionalNumbers.empty") }}</p>
    </section>
    <EditRegionalNumber ref="editModal" :regions="availableRegions" :species-id="species.id" @error="$emit('error', $event)" @saved="updated" />
    <RemoveRegionalNumber ref="removeModal" :species-id="species.id" @error="$emit('error', $event)" @removed="updated" />
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";

import EditRegionalNumber from "./EditRegionalNumber.vue";
import RegionalNumberCard from "./RegionalNumberCard.vue";
import RemoveRegionalNumber from "./RemoveRegionalNumber.vue";
import TarButton from "@/components/tar/TarButton.vue";
import type { RegionSummary } from "@/types/regions";
import type { RegionalNumber, Species, SpeciesFilters } from "@/types/species";
import { formatRegion } from "@/utils/format";
import { getSpeciesFilters } from "@/api/species";

const { t } = useI18n();

const props = defineProps<{
  species: Species;
}>();

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "updated", value: Species): void;
}>();

const editModal = ref<InstanceType<typeof EditRegionalNumber> | null>(null);
const regions = ref<RegionSummary[]>([]);
const removeModal = ref<InstanceType<typeof RemoveRegionalNumber> | null>(null);

const availableRegions = computed<RegionSummary[]>(() => {
  const used: Set<string> = new Set(props.species.regionalNumbers.map((entry) => entry.region.id));
  return regions.value.filter((region) => !used.has(region.id));
});
const regionalNumbers = computed<RegionalNumber[]>(() =>
  [...props.species.regionalNumbers].sort((a, b) => formatRegion(a.region).localeCompare(formatRegion(b.region))),
);

function add(): void {
  editModal.value?.open();
}
function edit(entry: RegionalNumber): void {
  editModal.value?.open(entry);
}
function remove(entry: RegionalNumber): void {
  removeModal.value?.open(entry);
}

function updated(species: Species): void {
  emit("updated", species);
}

onMounted(async () => {
  try {
    const filters: SpeciesFilters = await getSpeciesFilters();
    regions.value = filters.regions;
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>
