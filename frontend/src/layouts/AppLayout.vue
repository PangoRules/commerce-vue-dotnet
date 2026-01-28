<template>
  <div class="app-layout">
    <v-app-bar>
      <v-app-bar-title>{{ $t("app.name") }}</v-app-bar-title>
    </v-app-bar>

    <v-main>
      <!-- no container here unless you want global padding -->
      <div class="app-content">
        <slot />
      </div>
    </v-main>
  </div>
</template>

<style scoped>
.app-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}

.app-content {
  flex: 1;
  min-height: 0; /* IMPORTANT: enables proper scrolling */
  display: flex;
  flex-direction: column;
}
</style>
