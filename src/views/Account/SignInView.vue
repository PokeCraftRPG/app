<template>
  <main class="container">
    <h1>{{ t("account.signIn.title") }}</h1>
    <form @submit.prevent="submit">
      <EmailInput class="mb-3" required v-model="email" />
      <TarButton :disabled="isLoading" icon="fas fa-user" :loading="isLoading" :text="t('account.signIn.submit')" type="submit" />
    </form>
  </main>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useI18n } from "vue-i18n";

import EmailInput from "@/components/account/EmailInput.vue";
import TarButton from "@/components/tar/TarButton.vue";
import { type SignInAccountRequest } from "@/types/account";

const { locale, t } = useI18n();

const email = ref<string>("");
const isLoading = ref<boolean>(false);

async function submit(): Promise<void> {
  if (!isLoading.value) {
    isLoading.value = true;
    try {
      const payload: SignInAccountRequest = {
        credentials: {
          locale: locale.value,
          emailAddress: email.value,
          usePasswordless: false,
        },
      };
      console.log(payload); // TODO(fpion): implement
    } catch (e: unknown) {
      console.error(e); // TODO(fpion): implement
    } finally {
      isLoading.value = false;
    }
  }
}
</script>
