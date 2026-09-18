<template>
  <LinkCard :to="{ name: 'ItemEdit', params: { id: item.id } }">
    <div class="d-flex align-items-start gap-1">
      <ImageAsset v-if="item.sprite" :alt="t('sprite.alt', { name: formatItem(item) })" :asset="item.sprite" height="48" />
      <div class="flex-grow-1">
        <h5 class="card-title">{{ formatItem(item) }}</h5>
        <h6 class="card-subtitle mb-2 text-body-secondary">{{ category }}</h6>
      </div>
    </div>
    <div v-if="price || weight" class="d-flex justify-content-between align-items-center gap-2 mb-2 text-body-secondary">
      <div>
        <font-awesome-icon icon="fas fa-dollar-sign" aria-hidden="true" />&nbsp;<template v-if="item.price">{{ n(price, "money") }}</template
        ><span v-else class="text-secondary">{{ "—" }}</span>
      </div>
      <div>
        <font-awesome-icon icon="fas fa-weight-hanging" aria-hidden="true" />&nbsp;<template v-if="item.weight">{{ n(weight, "weight") }}</template
        ><span v-else class="text-secondary">{{ "—" }}</span>
      </div>
    </div>
    <div v-if="item.summary" class="card-text mb-2">{{ item.summary }}</div>
    <StatusBlock :actor="item.updatedBy" class="card-text small text-secondary" :date="item.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import ImageAsset from "@/components/shared/ImageAsset.vue";
import LinkCard from "@/components/shared/LinkCard.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Item } from "@/types/items";
import { formatItem } from "@/utils/format";
import { fromHundredths } from "@/utils/number";

const { n, t } = useI18n();

const props = defineProps<{
  item: Item;
}>();

const category = computed<string>(() => t(`items.category.options.${props.item.category}`));
const price = computed<number>(() => fromHundredths(props.item.price) ?? 0);
const weight = computed<number>(() => fromHundredths(props.item.weight) ?? 0);
</script>
