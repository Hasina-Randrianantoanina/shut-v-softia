import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getAuthHeaders } from "@/utils/authUtils";

const BASE_URL = "/api";

export const useAlertes = () => {
  return useQuery({
    queryKey: ["alertes"],
    queryFn: async () => {
      const response = await fetch(`${BASE_URL}/Alertes/Usage`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: "same-origin",
      });
      if (!response.ok) {
        throw new Error("Erreur " + response.statusText);
      }
      return response.json();
    },
  });
};

export const useAlertesObs = () => {
  return useQuery({
    queryKey: ["alertes"],
    queryFn: async () => {
      const response = await fetch(`${BASE_URL}/Alertes/Observation`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: 'same-origin',
      });
      if (!response.ok) {
        throw new Error("Erreur " + response.statusText);
      }
      return response.json();
    },
  });
};

export const useUpdateAlerteCommentaire = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, commentaire, utilisateurId }) => {
      const response = await fetch(`${BASE_URL}/Alertes/${id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: 'same-origin',
        body: JSON.stringify({ commentaire, utilisateurId }),
      });
      if (!response.ok) {
        throw new Error("Erreur lors de la mise à jour du commentaire");
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries(["alertes"]);
    },
  });
};
