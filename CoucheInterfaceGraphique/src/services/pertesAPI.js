import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getAuthHeaders } from "@/utils/authUtils";

const BASE_URL = "/api/Pertes";

export const usePertesByStation = (stationId) => {
  return useQuery({
    queryKey: ["pertes", stationId],
    queryFn: async () => {
      console.log(`Fetching pertes for station ${stationId}`);
      const response = await fetch(`${BASE_URL}/${stationId}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: "same-origin",
      });
      console.log(`Response status: ${response.status}`);
      if (!response.ok) {
        throw new Error("Erreur " + response.statusText);
      }
      const data = await response.json();
      console.log("Received data:", data);
      return data;
    },
    enabled: !!stationId,
  });
};

export const useUpdatePerte = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (perte) => {
      const response = await fetch(`${BASE_URL}/${perte.id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: "same-origin",
        body: JSON.stringify(perte),
      });
      if (!response.ok) {
        throw new Error("Erreur lors de la mise à jour");
      }
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries(["pertes", variables.stationId]);
    },
  });
};

export const useInsertPerte = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (perte) => {
      const response = await fetch(`${BASE_URL}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: "same-origin",
        body: JSON.stringify(perte),
      });
      if (!response.ok) {
        throw new Error("Erreur lors de l'insertion");
      }
      return response.json();
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries(["pertes", variables.stationId]);
    },
  });
};

export const useDeletePerte = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (perteId) => {
      const response = await fetch(`${BASE_URL}/${perteId}`, {
        method: "DELETE",
        headers: {
          ...getAuthHeaders(),
        },
        credentials: "same-origin",
      });
      if (!response.ok) {
        throw new Error("Erreur lors de la suppression");
      }
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries(["pertes"]);
    },
  });
};
