import { describe, it, expect, beforeEach } from "vitest";
import { useAuth } from "./useAuth";

describe("useAuth", () => {
  beforeEach(() => {
    const { logout } = useAuth();
    logout();
  });

  it("starts as unauthenticated", () => {
    const { isAuthenticated, user } = useAuth();
    expect(isAuthenticated.value).toBe(false);
    expect(user.value).toBeNull();
  });

  it("login sets user and isAuthenticated", () => {
    const { login, isAuthenticated, user } = useAuth();
    login();
    expect(isAuthenticated.value).toBe(true);
    expect(user.value).not.toBeNull();
    expect(user.value?.email).toBe("john.doe@example.com");
  });

  it("logout clears user and isAuthenticated", () => {
    const { login, logout, isAuthenticated, user } = useAuth();
    login();
    expect(isAuthenticated.value).toBe(true);
    logout();
    expect(isAuthenticated.value).toBe(false);
    expect(user.value).toBeNull();
  });

  it("toggleAuth switches between authenticated states", () => {
    const { toggleAuth, isAuthenticated } = useAuth();
    expect(isAuthenticated.value).toBe(false);
    toggleAuth();
    expect(isAuthenticated.value).toBe(true);
    toggleAuth();
    expect(isAuthenticated.value).toBe(false);
  });

  it("authState returns combined state", () => {
    const { login, authState } = useAuth();
    expect(authState.value.isAuthenticated).toBe(false);
    expect(authState.value.user).toBeNull();
    login();
    expect(authState.value.isAuthenticated).toBe(true);
    expect(authState.value.user?.name).toBe("John Doe");
  });

  it("shares state across multiple useAuth calls", () => {
    const auth1 = useAuth();
    const auth2 = useAuth();

    auth1.login();
    expect(auth2.isAuthenticated.value).toBe(true);
    expect(auth2.user.value?.email).toBe("john.doe@example.com");

    auth2.logout();
    expect(auth1.isAuthenticated.value).toBe(false);
  });
});
