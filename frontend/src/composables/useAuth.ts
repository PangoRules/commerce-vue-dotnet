import { ref, computed } from "vue";
import type { User, AuthState } from "@/types/api/authTypes";

const user = ref<User | null>(null);

const mockUser: User = {
  id: 1,
  email: "john.doe@example.com",
  name: "John Doe",
  avatar: null,
};

export function useAuth() {
  const isAuthenticated = computed(() => user.value !== null);

  const authState = computed<AuthState>(() => ({
    user: user.value,
    isAuthenticated: isAuthenticated.value,
  }));

  function login() {
    user.value = mockUser;
  }

  function logout() {
    user.value = null;
  }

  function toggleAuth() {
    if (isAuthenticated.value) {
      logout();
    } else {
      login();
    }
  }

  return {
    user,
    isAuthenticated,
    authState,
    login,
    logout,
    toggleAuth,
  };
}
