import { createI18n } from "vue-i18n";

import en from "./en";

type MessageSchema = typeof en;

export default createI18n<[MessageSchema], "en">({
  legacy: false,
  locale: "en",
  fallbackLocale: "en",
  messages: { en },
  datetimeFormats: {
    en: {
      medium: {
        year: "numeric",
        month: "short",
        day: "numeric",
        hour: "numeric",
        minute: "numeric",
        second: "numeric",
      },
    },
  },
  numberFormats: {
    en: {
      integer: {
        style: "decimal",
        minimumFractionDigits: 0,
        maximumFractionDigits: 0,
      },
      price: {
        style: "decimal",
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      },
      weight: {
        style: "decimal",
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      },
    },
  },
});
