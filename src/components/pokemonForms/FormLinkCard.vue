<template>
  <LinkCard :subtitle="subtitle" :title="title" :to="{ name: 'FormEdit', params: { id: form.id } }">
    <div class="d-flex align-items-center gap-2 mb-2">
      <PokemonTypeImage :type="form.types.primary" height="24" />
      <PokemonTypeImage v-if="form.types.secondary" :type="form.types.secondary" height="24" />
    </div>
    <div class="card-text mb-2">{{ category }}</div>
    <div class="card-text mb-2">{{ abilities }}</div>
    <div class="d-flex justify-content-between align-items-center gap-2 mb-2 text-body-secondary">
      <div>{{ n(height, "height") }}&nbsp;{{ t("unit.meter") }}</div>
      <div><font-awesome-icon icon="fas fa-weight-hanging" aria-hidden="true" />&nbsp;{{ n(weight, "height") }}&nbsp;{{ t("unit.kilogram") }}</div>
    </div>
    <div class="d-flex justify-content-between align-items-center gap-2 mb-2 text-body-secondary">
      <div>{{ t("forms.baseStatistics.total", { total: baseStatTotal }) }}</div>
      <div>{{ t("forms.yield.experience") }}&nbsp;{{ n(form.yield.experience, "integer") }}</div>
    </div>
    <div v-if="form.summary" class="card-text mb-2">{{ form.summary }}</div>
    <StatusBlock :actor="form.updatedBy" class="card-text small text-secondary" :date="form.updatedOn" relative />
  </LinkCard>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import LinkCard from "@/components/shared/LinkCard.vue";
import PokemonTypeImage from "@/components/pokemon/PokemonTypeImage.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Form } from "@/types/pokemonForms";
import { formatAbility, formatForm, formatVariety } from "@/utils/format";
import { fromTenths } from "@/utils/number";

const { n, t } = useI18n();

const props = defineProps<{
  form: Form;
}>();

const abilities = computed<string>(() =>
  [props.form.abilities.primary, props.form.abilities.secondary, props.form.abilities.hidden]
    .filter((ability) => Boolean(ability))
    .map((ability) => formatAbility(ability!))
    .join(" · "),
);
const baseStatTotal = computed<number>(
  () =>
    props.form.baseStatistics.hp +
    props.form.baseStatistics.attack +
    props.form.baseStatistics.defense +
    props.form.baseStatistics.specialAttack +
    props.form.baseStatistics.specialDefense +
    props.form.baseStatistics.speed,
);
const category = computed<string>(() => t(`forms.category.options.${props.form.category}`));
const height = computed<number>(() => fromTenths(props.form.size.height) ?? 0);
const subtitle = computed<string>(() => formatVariety(props.form.variety));
const title = computed<string>(() => formatForm(props.form));
const weight = computed<number>(() => fromTenths(props.form.size.weight) ?? 0);
</script>
