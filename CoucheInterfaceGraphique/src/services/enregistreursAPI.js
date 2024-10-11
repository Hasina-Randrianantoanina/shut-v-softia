import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getAuthHeaders } from "@/utils/authUtils";

const BASE_URL = "/api";

export const useEnregistreurs = () => {
  return useQuery({
    queryKey: ["enregistreurs"],
    queryFn: async () => {
      const response = await fetch(`${BASE_URL}/Enregistreurs`, {
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: "same-origin",
      });
      if (!response.ok) {
        throw new Error("Erreur lors de la récupération des enregistreurs");
      }
      return response.json();
    },
  });
};

export const useUpdateEnregistreur = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (updatedEnregistreur) => {
      const response = await fetch(
        `${BASE_URL}/Enregistreurs/${updatedEnregistreur.id}`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            ...getAuthHeaders(),
          },
          credentials: "same-origin",
          body: JSON.stringify(updatedEnregistreur),
        }
      );
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(
          errorData.message || "Erreur lors de la mise à jour de l'enregistreur"
        );
      }
      return response.json();
    },
    onSuccess: () => {
      queryClient.invalidateQueries([
        "enregistreurs",
        "stationsWithEnregistreur",
      ]);
    },
  });
};

export const useEnregistreurVersions = (liaison) => {
  return useQuery({
    queryKey: ["enregistreurVersions", liaison],
    queryFn: async () => {
      const response = await fetch(
        `${BASE_URL}/Enregistreurs/versions/${liaison}`,
        {
          headers: {
            "Content-Type": "application/json",
            ...getAuthHeaders(),
          },
          credentials: "same-origin",
        }
      );
      if (!response.ok) {
        throw new Error(
          "Erreur lors de la récupération des versions d'enregistreurs"
        );
      }
      return response.json();
    },
    enabled: !!liaison,
  });
};

export const useUpdateEnregistreurDernierTransfert = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ enregistreurId, dernierTransfert }) => {
      const response = await fetch(
        `${BASE_URL}/Enregistreurs/${enregistreurId}/dernierTransfert`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            ...getAuthHeaders(),
          },
          credentials: "same-origin",
          body: JSON.stringify(dernierTransfert),
        }
      );
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(
          errorData.message ||
            "Erreur lors de la mise à jour de la date de dernier transfert"
        );
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries(["pertes"]);
    },
  });
};
