<template>
  <LinkCard :to="{ name: 'Trainer', params: { id: trainer.id } }">
    <div class="d-flex align-items-start gap-1">
      <ImageAsset v-if="trainer.sprite" :alt="t('sprite.alt', { name: trainer.name ?? trainer.key })" :asset="trainer.sprite" height="48" />
      <div class="flex-grow-1">
        <h5 class="card-title">
          <template v-if="trainer.gender"><GenderIcon :gender="trainer.gender" />&nbsp;</template>{{ trainer.name ?? trainer.key }}
        </h5>
        <h6 v-if="trainer.license" class="card-subtitle mb-2 text-body-secondary">{{ trainer.license }}</h6>
      </div>
    </div>
    <div class="d-flex justify-content-between align-items-center gap-2 mb-2 text-body-secondary">
      <div><font-awesome-icon icon="fas fa-sack-dollar" aria-hidden="true" />&nbsp;{{ n(money, "money") }}</div>
      <div>
        <font-awesome-icon icon="fas fa-dog" aria-hidden="true" />&nbsp;{{
          t("trainers.party.format", { count: n(trainer.partyCount, "integer"), limit: n(partyLimit, "integer") })
        }}
      </div>
    </div>
    <div v-if="trainer.summary" class="card-text">{{ trainer.summary }}</div>
    <StatusBlock :actor="trainer.updatedBy" class="card-text mt-2 small text-secondary" :date="trainer.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import GenderIcon from "@/components/shared/GenderIcon.vue";
import ImageAsset from "@/components/shared/ImageAsset.vue";
import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Trainer } from "@/types/trainers";
import { fromHundredths } from "@/utils/number";

const { n, t } = useI18n();

const props = defineProps<{
  trainer: Trainer;
}>();

const money = computed<number>(() => fromHundredths(props.trainer.money) ?? 0);
const partyLimit = computed<number>(() => props.trainer.partyLimit ?? 6);
</script>
