<template>
  <LinkCard :subtitle="subtitle" :title="title" :to="{ name: 'FormEdit', params: { id: form.id } }">
    <div class="d-flex justify-content-between align-items-center gap-2 mb-2">
      <PokemonTypeImage :type="form.types.primary" height="24" />
      <PokemonTypeImage v-if="form.types.secondary" :type="form.types.secondary" height="24" />
    </div>
    <div class="mb-2">
      <div class="d-flex justify-content-between align-items-center gap-2 text-body-secondary">
        <div>{{ formatAbility(form.abilities.primary) }}</div>
        <div v-if="form.abilities.secondary">{{ formatAbility(form.abilities.secondary) }}</div>
        <div v-if="form.abilities.hidden"><font-awesome-icon icon="fas fa-eye-slash" aria-hidden="true" />&nbsp;{{ formatAbility(form.abilities.hidden) }}</div>
      </div>
      <div v-if="height || weight" class="d-flex justify-content-between align-items-center gap-2 text-body-secondary">
        <div>
          <font-awesome-icon icon="fas fa-ruler-vertical" aria-hidden="true" />&nbsp;<template v-if="height"
            >{{ n(height, "pokemonHeight") }}&nbsp;{{ t("unit.meter") }}</template
          ><span v-else class="text-secondary">{{ "—" }}</span>
        </div>
        <div>
          <font-awesome-icon icon="fas fa-weight-hanging" aria-hidden="true" />&nbsp;<template v-if="weight"
            >{{ n(weight, "pokemonWeight") }}&nbsp;{{ t("unit.kilogram") }}</template
          ><span v-else class="text-secondary">{{ "—" }}</span>
        </div>
      </div>
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
import { formatAbility, formatForm } from "@/utils/format";
import { fromTenths } from "@/utils/number";

const { n, t } = useI18n();

const props = defineProps<{
  form: Form;
}>();

const height = computed<number>(() => fromTenths(props.form.size.height) ?? 0);
const subtitle = computed<string>(() => t(`forms.category.options.${props.form.category}`));
const title = computed<string>(() => formatForm(props.form));
const weight = computed<number>(() => fromTenths(props.form.size.weight) ?? 0);
</script>
