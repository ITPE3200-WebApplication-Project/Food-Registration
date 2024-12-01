import { jwtDecode } from "jwt-decode";
import { api } from "./api";

interface AuthService {
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
  register: (
    email: string,
    password: string,
    confirmPassword: string
  ) => Promise<string>;
  getUser: () => {
    email: string | null;
    loggedIn: boolean;
  };
}

export const authService: AuthService = {
  login: async (email: string, password: string) => {
    try {
      const response = await api.post("/auth/login", { email, password });
      const { token } = response.data;
      localStorage.setItem("token", token);
      // eslint-disable-next-line @typescript-eslint/no-unused-vars
    } catch (error) {
      throw new Error("Authentication failed");
    }
  },

  logout: () => {
    localStorage.removeItem("token");
    window.location.href = "/login";
  },

  register: async (
    email: string,
    password: string,
    confirmPassword: string
  ) => {
    try {
      const response = await api.post("/auth/register", {
        email,
        password,
        confirmPassword,
      });
      const { token } = response.data;
      localStorage.setItem("token", token);
      return token;
      // eslint-disable-next-line @typescript-eslint/no-unused-vars
    } catch (error) {
      throw new Error("Registration failed");
    }
  },

  getUser: () => {
    const token = localStorage.getItem("token");
    if (!token) return { email: null, loggedIn: false };

    const decodedToken = jwtDecode(token) as {
      "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress": string;
      exp: number;
    };

    // Get expiration date
    const expirationDate = decodedToken.exp;
    // Check if token is expired
    if (expirationDate < Date.now() / 1000) {
      localStorage.removeItem("token");
      return { email: null, loggedIn: false };
    }

    return {
      email:
        decodedToken[
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
        ],
      loggedIn: true,
    };
  },
};
