<template>
  <TarImage :alt="alt" :height="height" :src="src" />
</template>

<script setup lang="ts">
import { computed } from "vue";

import TarImage from "@/components/tar/TarImage.vue";
import type { Asset } from "@/types/assets";
import { useWorldStore } from "@/stores/world";

const apiBaseUrl: string = import.meta.env.VITE_APP_API_BASE_URL ?? "";
const world = useWorldStore();

const props = defineProps<{
  alt?: string;
  asset: Asset;
  height?: number | string;
}>();

function formatId(id: string): string {
  return id.replaceAll("-", "").toLowerCase();
}

const src = computed<string>(() => {
  const parts: string[] = [`${apiBaseUrl}/assets`];
  if (world.current) {
    parts.push(formatId(world.current.id));
  }
  parts.push("image");
  parts.push(`${formatId(props.asset.id)}.${props.asset.file.extension}`);
  return parts.join("/");
});
</script>
