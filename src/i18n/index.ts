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
      itemWeight: {
        style: "decimal",
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      },
      money: {
        style: "decimal",
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
      },
      pokemonHeight: {
        style: "decimal",
        minimumFractionDigits: 1,
        maximumFractionDigits: 1,
      },
      pokemonNumber: {
        style: "decimal",
        minimumIntegerDigits: 4,
        maximumFractionDigits: 0,
        useGrouping: false,
      },
      pokemonWeight: {
        style: "decimal",
        minimumFractionDigits: 1,
        maximumFractionDigits: 1,
      },
    },
  },
});
