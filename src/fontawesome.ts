import type { App } from "vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

import { library } from "@fortawesome/fontawesome-svg-core";
import {
  faAdjust,
  faArrowLeft,
  faArrowRight,
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faArrowUp,
  faBan,
  faCheck,
  faClockRotateLeft,
  faCommentSms,
  faDesktop,
  faDice,
  faDungeon,
  faEdit,
  faEnvelope,
  faFloppyDisk,
  faHatWizard,
  faHome,
  faKey,
  faMobile,
  faMoon,
  faNetworkWired,
  faPlus,
  faRobot,
  faSun,
  faTablet,
  faUser,
  faXmark,
} from "@fortawesome/free-solid-svg-icons";

library.add(
  faAdjust,
  faArrowLeft,
  faArrowRight,
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faArrowUp,
  faBan,
  faCheck,
  faClockRotateLeft,
  faCommentSms,
  faDesktop,
  faDice,
  faDungeon,
  faEdit,
  faEnvelope,
  faFloppyDisk,
  faHatWizard,
  faHome,
  faKey,
  faMobile,
  faMoon,
  faNetworkWired,
  faPlus,
  faRobot,
  faSun,
  faTablet,
  faUser,
  faXmark,
);

export default function (app: App) {
  app.component("font-awesome-icon", FontAwesomeIcon);
}
