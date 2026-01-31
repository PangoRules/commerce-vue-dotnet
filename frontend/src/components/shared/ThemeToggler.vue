<template>
  <button
    class="theme-toggle"
    :class="{ 'theme-toggle--toggled': isDark }"
    type="button"
    :title="isDark ? 'Switch to light mode' : 'Switch to dark mode'"
    :aria-label="isDark ? 'Switch to light mode' : 'Switch to dark mode'"
    @click="toggle"
  >
    <svg
      xmlns="http://www.w3.org/2000/svg"
      aria-hidden="true"
      width="1em"
      height="1em"
      class="theme-toggle__icon"
      fill="currentColor"
      viewBox="0 0 32 32"
      :style="iconStyle"
    >
      <clipPath :id="clipId">
        <path
          d="M0-5h55v37h-55zm32 12a1 1 0 0025 0 1 1 0 00-25 0"
          :style="cutoutStyle"
        />
      </clipPath>
      <g :clip-path="`url(#${clipId})`">
        <circle cx="16" cy="16" r="15" />
      </g>
    </svg>
  </button>
</template>

<script setup lang="ts">
import { computed } from "vue";
import { useThemeStore } from "@/stores/theme";

const themeStore = useThemeStore();

const isDark = computed(() => themeStore.mode === "dark");

// Unique ID to avoid conflicts if multiple togglers are rendered
const clipId = `theme-toggle-cutout-${Math.random().toString(36).slice(2, 9)}`;

const iconStyle = computed(() => ({
  transform: isDark.value ? "rotate(180deg)" : "rotate(0deg)",
  transition: "transform 0.5s cubic-bezier(0.4, 0, 0.2, 1)",
}));

const cutoutStyle = computed(() => ({
  transform: isDark.value ? "translateX(-20px)" : "translateX(0)",
  transition: "transform 0.5s cubic-bezier(0.4, 0, 0.2, 1)",
}));

function toggle() {
  themeStore.toggleMode();
}
</script>

<style scoped>
.theme-toggle {
  --size: 1.5rem;

  background: none;
  border: none;
  padding: 0.5rem;
  cursor: pointer;
  font-size: var(--size);
  color: currentColor;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 50%;
  transition: background-color 0.2s ease;
}

.theme-toggle:hover {
  background-color: rgba(128, 128, 128, 0.15);
}

.theme-toggle:focus-visible {
  outline: 2px solid currentColor;
  outline-offset: 2px;
}
</style>
