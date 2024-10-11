import { jwtDecode } from 'jwt-decode';

export const setToken = (token) => {
  localStorage.setItem('token', token);
};

export const getToken = () => {
  return localStorage.getItem('token');
};

export const removeToken = () => {
  localStorage.removeItem('token');
};

export const isAuthenticated = () => {
  const token = getToken();
  if (!token) return false;
  
  try {
    const decodedToken = jwtDecode(token);
    const currentTime = Date.now() / 1000;
    return decodedToken.exp > currentTime;
  } catch (error) {
    return false;
  }
};

export const getAuthHeaders = () => {
  const token = getToken();
  return token ? { 'Authorization': `Bearer ${token}` } : {};
};

export const getUserRole = () => {
  const token = getToken();
  if (!token) return null;

  try {
    const decodedToken = jwtDecode(token);
    return decodedToken.role;
  } catch (error) {
    return null;
  }
};

export const getUserInfo = () => {
  const token = getToken();
  if (!token) return null;

  try {
    const decodedToken = jwtDecode(token);
    return {
      id: parseInt(decodedToken.nameid) || 0,
      nom: decodedToken.unique_name,
      email: decodedToken.email,
      profilId: parseInt(decodedToken.ProfilId) || 0,
      profilName: decodedToken.ProfilName || "Unknown",
      profilCode: parseInt(decodedToken.ProfilCode) || 0,
      role: decodedToken.role || decodedToken.ProfilName || "Unknown"
    };
  } catch (error) {
    console.error("Erreur lors du décodage du token:", error);
    return null;
  }
};

export const logout = () => {
  removeToken();
  localStorage.removeItem('userRole');  // Ajout de cette ligne
  sessionStorage.removeItem('user');
  sessionStorage.removeItem('sessionExpiration');
  // Déclencher un événement de stockage pour informer les autres onglets/fenêtres
  window.dispatchEvent(new Event('storage'));
};