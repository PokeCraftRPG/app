<template>
  <LinkCard :subtitle="subtitle" :title="title" :to="{ name: 'VarietyEdit', params: { id: variety.id } }">
    <div class="d-flex justify-content-between align-items-center gap-2 mb-2">
      <div>
        <DefaultBadge v-if="variety.isDefault" />
      </div>
      <div>{{ genderRatio }}</div>
    </div>
    <div v-if="variety.summary" class="card-text mb-2">{{ variety.summary }}</div>
    <StatusBlock :actor="variety.updatedBy" class="card-text small text-secondary" :date="variety.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import DefaultBadge from "./DefaultBadge.vue";
import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Variety } from "@/types/varieties";
import { formatVariety } from "@/utils/format";

const { t } = useI18n();

const props = defineProps<{
  variety: Variety;
}>();

const genderRatio = computed<string>(() =>
  t(typeof props.variety.genderRatio === "number" ? `varieties.genderRatio.options.${props.variety.genderRatio}` : "varieties.genderRatio.options.placeholder"),
);
const subtitle = computed<string>(() => props.variety.genus ?? "");
const title = computed<string>(() => formatVariety(props.variety));
</script>
