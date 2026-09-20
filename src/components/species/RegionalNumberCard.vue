<template>
  <div class="position-relative h-100">
    <TarCard class="clickable h-100" @click="$emit('click')">
      <h5 class="card-title">{{ formatPokemonNumber(regionalNumber.number, n) }}</h5>
      <h6 class="card-subtitle mb-0 text-body-secondary">{{ formatRegion(regionalNumber.region) }}</h6>
    </TarCard>
    <div class="position-absolute top-0 end-0 m-2" @click.stop>
      <button type="button" class="btn btn-sm" @click="$emit('remove')">
        <font-awesome-icon icon="fas fa-xmark" aria-hidden="true" />
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useI18n } from "vue-i18n";

import TarCard from "@/components/tar/TarCard.vue";
import type { RegionalNumber } from "@/types/species";
import { formatPokemonNumber, formatRegion } from "@/utils/format";

const { n } = useI18n();

defineProps<{
  regionalNumber: RegionalNumber;
}>();

defineEmits<{
  (e: "click"): void;
  (e: "remove"): void;
}>();
</script>
