<template>
  <font-awesome-icon :icon="icon" />
</template>

<script setup lang="ts">
import { computed } from "vue";

import type { Gender } from "@/types/trainers";

const props = defineProps<{
  gender?: Gender | null;
}>();

const icon = computed<string>(() => {
  switch (props.gender) {
    case "Female":
      return "fas fa-venus";
    case "Male":
      return "fas fa-mars";
    default:
      return "fas fa-question";
  }
});
</script>
