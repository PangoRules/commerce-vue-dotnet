export type User = {
  id: number;
  email: string;
  name: string;
  avatar?: string | null;
};

export type AuthState = {
  user: User | null;
  isAuthenticated: boolean;
};
