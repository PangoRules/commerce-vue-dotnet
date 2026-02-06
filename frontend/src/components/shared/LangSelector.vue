<script setup lang="ts">
import { useAppStore } from "@/stores/app";
import { supportedLocales } from "@/i18n";
import type { Locales } from "@/i18n/i18types";

const appStore = useAppStore();

const setLocale = (locale: Locales) => {
  appStore.setLocale(locale);
};
</script>

<template>
  <v-menu open-on-hover>
    <template v-slot:activator="{ props }">
      <v-btn color="primary" v-bind="props"> {{ appStore.locale }} </v-btn>
    </template>

    <v-list>
      <v-list-item
        v-for="(item, index) in supportedLocales"
        :key="index"
        :value="item"
      >
        <v-list-item-title @click="setLocale(item as Locales)">{{
          item.toUpperCase()
        }}</v-list-item-title>
      </v-list-item>
    </v-list>
  </v-menu>
</template>
