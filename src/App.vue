<template>
  <div class="d-flex flex-column min-vh-100">
    <LayoutNavbar />
    <div class="flex-grow-1 py-3">
      <RouterView />
      <!-- TODO(fpion): <Transition name="fade">
         <button
          v-if="showScrollTop"
          type="button"
          class="btn btn-lg btn-primary btn-scroll position-fixed end-0 m-3 rounded-circle shadow"
          :style="{ bottom: `${footerOverlap}px` }"
          @click="scrollToTop"
        >
          <font-awesome-icon icon="fas fa-arrow-up" />
        </button>
      </Transition> -->
    </div>
    <LayoutFooter id="footer" />
  </div>
</template>

<script setup lang="ts">
import { RouterView } from "vue-router";
import { handleErrorKey } from "./inject";
import { provide } from "vue";

import LayoutFooter from "./components/layout/LayoutFooter.vue";
import LayoutNavbar from "./components/layout/LayoutNavbar.vue";

function handleError(e: unknown): void {
  if (e) {
    console.error(e);
  }
  // TODO(fpion): toasts.error();
}
provide(handleErrorKey, handleError);
</script>
