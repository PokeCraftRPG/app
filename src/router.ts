import { createRouter, createWebHistory } from "vue-router";

import HomeView from "./views/HomeView.vue";
import { useAccountStore } from "./stores/account";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/",
      name: "Home",
      component: HomeView,
      meta: { isPublic: true },
    },
    // Account
    {
      name: "Profile",
      path: "/profile",
      component: () => import("./views/account/ProfileView.vue"),
      meta: { isPublic: true },
    },
    {
      name: "SignIn",
      path: "/auth",
      component: () => import("./views/account/SignInView.vue"),
      meta: { isPublic: true },
    },
    {
      name: "SignOut",
      path: "/logout",
      component: () => import("./views/account/SignOutView.vue"),
      meta: { isPublic: true },
    },
    // Worlds
    {
      name: "Worlds",
      path: "/worlds",
      component: () => import("./views/worlds/WorldsView.vue"),
    },
    {
      name: "World",
      path: "/worlds/:id",
      component: () => import("./views/worlds/WorldView.vue"),
    },
    // Abilities
    {
      name: "Abilities",
      path: "/abilities",
      component: () => import("./views/abilities/AbilitiesView.vue"),
    },
    {
      name: "AbilityCreate",
      path: "/abilities/new",
      component: () => import("./views/abilities/AbilityCreateView.vue"),
    },
    {
      name: "AbilityEdit",
      path: "/abilities/:id",
      component: () => import("./views/abilities/AbilityEditView.vue"),
    },
    // Items
    {
      name: "Items",
      path: "/items",
      component: () => import("./views/items/ItemsView.vue"),
    },
    {
      name: "ItemCreate",
      path: "/items/new",
      component: () => import("./views/items/ItemCreateView.vue"),
    },
    {
      name: "ItemEdit",
      path: "/items/:id",
      component: () => import("./views/items/ItemEditView.vue"),
    },
    // Moves
    {
      name: "Moves",
      path: "/moves",
      component: () => import("./views/moves/MovesView.vue"),
    },
    {
      name: "MoveCreate",
      path: "/moves/new",
      component: () => import("./views/moves/MoveCreateView.vue"),
    },
    {
      name: "MoveEdit",
      path: "/moves/:id",
      component: () => import("./views/moves/MoveEditView.vue"),
    },
    // Regions
    {
      name: "Regions",
      path: "/regions",
      component: () => import("./views/regions/RegionsView.vue"),
    },
    {
      name: "RegionCreate",
      path: "/regions/new",
      component: () => import("./views/regions/RegionCreateView.vue"),
    },
    {
      name: "RegionEdit",
      path: "/regions/:id",
      component: () => import("./views/regions/RegionEditView.vue"),
    },
    // Species
    {
      name: "Species",
      path: "/species",
      component: () => import("./views/species/SpeciesView.vue"),
    },
    {
      name: "SpeciesDetail",
      path: "/species/:id",
      component: () => import("./views/species/SpeciesDetailView.vue"),
    },
    // Trainers
    {
      name: "Trainers",
      path: "/trainers",
      component: () => import("./views/trainers/TrainersView.vue"),
    },
    {
      name: "TrainerCreate",
      path: "/trainers/new",
      component: () => import("./views/trainers/TrainerCreateView.vue"),
    },
    {
      name: "TrainerEdit",
      path: "/trainers/:id",
      component: () => import("./views/trainers/TrainerEditView.vue"),
    },
    // NotFound
    {
      name: "NotFound",
      path: "/:pathMatch(.*)*",
      component: () => import("./views/NotFound.vue"),
      // route level code-splitting
      // this generates a separate chunk (NotFound.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      meta: { isPublic: true },
    },
  ],
});

router.beforeEach(async (to) => {
  const account = useAccountStore();
  if (!to.meta.isPublic && !account.currentUser) {
    return { name: "SignIn", query: { redirect: to.fullPath } };
  }
});

export default router;
