import { useEffect, useCallback } from 'react';
import { useRouter } from 'next/router';

const SESSION_TIMEOUT = 2 * 60 * 60 * 1000; // 2 heures
const ACTIVITY_EXTENSION = 5 * 60 * 1000; // 5 minutes

export const useSessionTimeout = () => {
  const router = useRouter();

  const initializeTimeout = useCallback(() => {
    const expiresAt = Date.now() + SESSION_TIMEOUT;
    sessionStorage.setItem('sessionExpiration', expiresAt.toString());
  }, []);

  const extendTimeout = useCallback(() => {
    const currentExpiration = sessionStorage.getItem('sessionExpiration');
    if (currentExpiration) {
      const newExpiration = Math.min(
        parseInt(currentExpiration, 10) + ACTIVITY_EXTENSION,
        Date.now() + SESSION_TIMEOUT
      );
      sessionStorage.setItem('sessionExpiration', newExpiration.toString());
    } else {
      initializeTimeout();
    }
  }, [initializeTimeout]);

  const checkTimeout = useCallback(() => {
    const expirationTime = sessionStorage.getItem('sessionExpiration');
    if (expirationTime && Date.now() > parseInt(expirationTime, 10)) {
      sessionStorage.removeItem('user');
      sessionStorage.removeItem('sessionExpiration');
      router.push('/login');
    }
  }, [router]);

  useEffect(() => {
    const handleActivity = () => {
      checkTimeout();
      extendTimeout();
    };

    // Initialiser le timeout
    initializeTimeout();

    // Actions pour étendre le timeout
    window.addEventListener('mousemove', handleActivity);
    window.addEventListener('keydown', handleActivity);

    // Vérifier le timeout toutes les minutes
    const intervalId = setInterval(checkTimeout, 60 * 1000);

    return () => {
      window.removeEventListener('mousemove', handleActivity);
      window.removeEventListener('keydown', handleActivity);
      clearInterval(intervalId);
    };
  }, [checkTimeout, extendTimeout, initializeTimeout]);

  return initializeTimeout;
};