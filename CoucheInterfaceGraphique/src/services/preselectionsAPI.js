import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getAuthHeaders } from "@/utils/authUtils";

const BASE_URL = "/api/preselections";

export const useGetPreselections = () => {
  return useQuery({
    queryKey: ["preselections"],
    queryFn: async () => {
      const response = await fetch(`${BASE_URL}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: "same-origin",
      });
      if (!response.ok) {
        throw new Error("Erreur lors de la récupération des présélections");
      }
      return response.json();
    },
  });
};

export const useGetPreselectionById = (id) => {
  return useQuery({
    queryKey: ["preselection", id],
    queryFn: async () => {
      const response = await fetch(`${BASE_URL}/${id}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: "same-origin",
      });
      if (!response.ok) {
        throw new Error(`Erreur lors de la récupération de la présélection avec l'ID ${id}`);
      }
      return response.json();
    },
    enabled: !!id,
  });
};

export const useInsertPreselection = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (newPreselection) => {
      const response = await fetch(`${BASE_URL}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: "same-origin",
        body: JSON.stringify(newPreselection),
      });
      if (!response.ok) {
        throw new Error("Erreur lors de l'insertion de la nouvelle présélection");
      }
      return response.json();
    },
    onSuccess: () => {
      queryClient.invalidateQueries("preselections");
    },
  });
};