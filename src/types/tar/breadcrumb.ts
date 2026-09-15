import type { RouteLocationAsPathGeneric, RouteLocationAsRelativeGeneric } from "vue-router";

export type Breadcrumb = {
  text: string;
  to?: string | RouteLocationAsRelativeGeneric | RouteLocationAsPathGeneric;
};

export type BreadcrumbOptions = {
  ariaLabel?: string;
  breadcrumbs?: Breadcrumb[];
  divider?: string;
};
