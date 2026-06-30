import { createContext, useContext, useEffect, useState } from "react";
import { API_URL } from "../config";

const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);

  useEffect(() => {
    const savedUser = localStorage.getItem("autoservisUser");

    if (savedUser) {
      setUser(JSON.parse(savedUser));
    }
  }, []);

  async function login(email, password) {
    try {
      const response = await fetch(`${API_URL}/api/users/login`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ email, password }),
      });

      const data = await response.json();

      if (!response.ok) {
        return {
          success: false,
          message: data.message || "Prijava nije uspela.",
        };
      }

      localStorage.setItem("autoservisUser", JSON.stringify(data));
      setUser(data);

      return {
        success: true,
      };
    } catch (error) {
      return {
        success: false,
        message: "Greška pri povezivanju sa serverom.",
      };
    }
  }

  async function register(name, email, password) {
    try {
      const response = await fetch(`${API_URL}/api/users/register`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({ name, email, password }),
      });

      const data = await response.json();

      if (!response.ok) {
        return {
          success: false,
          message: data.message || "Registracija nije uspela.",
        };
      }

      localStorage.setItem("autoservisUser", JSON.stringify(data));
      setUser(data);

      return {
        success: true,
      };
    } catch (error) {
      return {
        success: false,
        message: "Greška pri povezivanju sa serverom.",
      };
    }
  }

  async function updateProfile(profileData) {
    try {
      const response = await fetch(`${API_URL}/api/users/profile`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${user.token}`,
        },
        body: JSON.stringify(profileData),
      });

      const data = await response.json();

      if (!response.ok) {
        return {
          success: false,
          message: data.message || "Izmena profila nije uspela.",
        };
      }

      localStorage.setItem("autoservisUser", JSON.stringify(data));
      setUser(data);

      return {
        success: true,
      };
    } catch (error) {
      return {
        success: false,
        message: "Greška pri povezivanju sa serverom.",
      };
    }
  }

  function logout() {
    localStorage.removeItem("autoservisUser");
    setUser(null);
  }

  return (
    <AuthContext.Provider
      value={{ user, login, register, updateProfile, logout }}
    >
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}