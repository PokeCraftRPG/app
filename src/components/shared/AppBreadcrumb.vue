<template>
  <nav :aria-label="ariaLabel ? t(ariaLabel) : undefined" :style="divider ? { '--bs-breadcrumb-divider': `'${divider}'` } : undefined">
    <ol class="breadcrumb">
      <li v-for="(breadcrumb, index) in breadcrumbs" :key="index" :class="getClasses(breadcrumb)" :aria-current="getAriaCurrent(breadcrumb)">
        <RouterLink v-if="breadcrumb.to" :to="breadcrumb.to">{{ breadcrumb.text }}</RouterLink>
        <template v-else>{{ breadcrumb.text }}</template>
      </li>
    </ol>
  </nav>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { Breadcrumb } from "@/types/components";

const { t } = useI18n();

const props = withDefaults(
  defineProps<{
    active?: string;
    ariaLabel?: string;
    divider?: string;
    parent?: Breadcrumb[];
  }>(),
  {
    divider: "›",
    parent: () => [],
  },
);

const breadcrumbs = computed<Breadcrumb[]>(() => {
  const breadcrumbs: Breadcrumb[] = [];
  props.parent.forEach((breadcrumb) => breadcrumbs.push(breadcrumb));
  if (props.active) {
    breadcrumbs.push({ text: props.active });
  }
  if (breadcrumbs.length) {
    breadcrumbs.splice(0, 0, { text: t("home.title"), to: { name: "Home" } });
  } else {
    breadcrumbs.push({ text: t("home.title") });
  }
  return breadcrumbs;
});

function getAriaCurrent(breadcrumb: Breadcrumb): "page" | undefined {
  return breadcrumb.to ? undefined : "page";
}
function getClasses(breadcrumb: Breadcrumb): string[] {
  const classes = ["breadcrumb-item"];
  if (!breadcrumb.to) {
    classes.push("active");
  }
  return classes;
}
</script>
