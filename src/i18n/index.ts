import { createI18n } from "vue-i18n";

import en from "./en";

type MessageSchema = typeof en;

export default createI18n<[MessageSchema], "en">({
  legacy: false,
  locale: "en",
  fallbackLocale: "en",
  messages: { en },
});
