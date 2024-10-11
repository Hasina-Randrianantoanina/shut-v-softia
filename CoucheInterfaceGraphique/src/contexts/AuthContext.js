import React, { createContext, useContext, useState, useEffect } from 'react';
import { getToken, getUserInfo, isAuthenticated } from '@/utils/authUtils';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const updateUserInfo = () => {
      setLoading(true);
      if (isAuthenticated()) {
        const userInfo = getUserInfo();
        if (userInfo) {
          setUser(userInfo);
        } else {
          setUser(null);
        }
      } else {
        setUser(null);
      }
      setLoading(false);
    };

    updateUserInfo();
    window.addEventListener('storage', updateUserInfo);

    return () => {
      window.removeEventListener('storage', updateUserInfo);
    };
  }, []);

  const login = (userData) => {
    setUser(userData);
  };

  const logout = () => {
    setUser(null);
  };

  return (
    <AuthContext.Provider value={{ user, setUser, login, logout, loading }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);