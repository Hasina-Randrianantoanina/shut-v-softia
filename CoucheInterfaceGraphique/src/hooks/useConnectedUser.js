import { useState, useEffect } from 'react';
import { getUserInfo } from '@/utils/authUtils';

export const useConnectedUser = () => {
  const [connectedUser, setConnectedUser] = useState("");

  useEffect(() => {
    const updateUser = () => {
      const userInfo = getUserInfo();
      if (userInfo) {
        setConnectedUser(userInfo.nom || userInfo.unique_name || "");
      } else {
        setConnectedUser("");
      }
    };

    updateUser();

    window.addEventListener('userLoggedIn', updateUser);
    window.addEventListener('storage', updateUser);

    return () => {
      window.removeEventListener('userLoggedIn', updateUser);
      window.removeEventListener('storage', updateUser);
    };
  }, []);

  return connectedUser;
};