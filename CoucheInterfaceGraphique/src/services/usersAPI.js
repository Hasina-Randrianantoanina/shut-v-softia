import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getAuthHeaders, setToken } from "@/utils/authUtils";

const BASE_URL = "/api";

export const useUsers = () => {
  return useQuery({
    queryKey: ["users"],
    queryFn: async () => {
      const response = await fetch(`${BASE_URL}/Users`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: 'same-origin',
      });
      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        throw new Error(errorData.message || "Erreur lors de la récupération des utilisateurs");
      }
      return response.json();
    },
  });
};

export const useLogin = () => {
  return useMutation({
    mutationFn: async (credentials) => {
      const response = await fetch(`${BASE_URL}/Authentication/User`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          Mail: credentials.Mail,
          Password: credentials.Password,
        }),
        credentials: 'same-origin',
      });
      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        throw new Error(errorData.message || "Erreur authentification");
      }
      const data = await response.json();
      setToken(data.token);
      return data;
    },
  });
};

export const useAddUser = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (user) => {
      const response = await fetch(`${BASE_URL}/Users`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        body: JSON.stringify(user),
        credentials: 'same-origin',
      });
      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        throw new Error(errorData.message || "Erreur lors de l'ajout de l'utilisateur");
      }
      return response.json();
    },
    onSuccess: () => {
      queryClient.invalidateQueries(["users"]);
    },
  });
};

export const useUpdateUser = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (user) => {
      const response = await fetch(`${BASE_URL}/Users/${user.id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        body: JSON.stringify(user),
        credentials: 'same-origin',
      });
      if (!response.ok) {
        let errorMessage = "Erreur lors de la mise à jour de l'utilisateur";
        try {
          const errorData = await response.json();
          errorMessage = errorData.message || errorMessage;
        } catch (parseError) {
          console.error("Erreur lors de la lecture de la réponse d'erreur:", parseError);
        }
        throw new Error(errorMessage);
      }
      try {
        return await response.json();
      } catch (parseError) {
        console.warn("La réponse n'est pas du JSON valide, mais la mise à jour a peut-être réussi");
        return { message: "Mise à jour réussie, mais la réponse n'est pas du JSON" };
      }
    },
    onSuccess: (data, variables) => {
      queryClient.setQueryData(["users"], (oldData) => {
        if (!oldData) return oldData;
        return oldData.map(user => user.id === variables.id ? { ...user, ...variables } : user);
      });
    },
  });
};

export const useDeleteUser = () => {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: async (userId) => {
      const response = await fetch(`${BASE_URL}/Users/${userId}`, {
        method: "DELETE",
        headers: {
          ...getAuthHeaders(),
        },
        credentials: 'same-origin',
      });
      if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        throw new Error(errorData.message || "Erreur lors de la suppression de l'utilisateur");
      }
      return response.json();
    },
    onSuccess: () => {
      queryClient.invalidateQueries(["users"]);
    },
  });
};