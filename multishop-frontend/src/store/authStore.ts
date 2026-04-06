import { User } from "../types/auth";

const USER_KEY = "user";

export const authStore = {
  getUser(): User | null {
    const data = localStorage.getItem(USER_KEY);
    return data ? JSON.parse(data) : null;
  },

  setUser(user: User): void {
    localStorage.setItem(USER_KEY, JSON.stringify(user));
  },

  removeUser(): void {
    localStorage.removeItem(USER_KEY);
  },

  isLoggedIn(): boolean {
    return !!localStorage.getItem(USER_KEY);
  },
};
