import { getAuthHeaders } from "@/utils/authUtils";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { userHasAccessToVoie } from '@/utils/abonnementsUtils';
import { useAuth } from '@/contexts/AuthContext';

const BASE_URL = "/api";

export const useAddVoieTelemesuree = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: async (newVoieTelemesuree) => {
      const response = await fetch(`${BASE_URL}/Voies/telemesuree`, {
        method: 'POST',
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: 'same-origin',
        body: JSON.stringify(newVoieTelemesuree),
      });
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Erreur lors de l\'ajout de la voie télémesurée');
      }
      return response.json();
    },
    onSuccess: (data, variables) => {
      queryClient.invalidateQueries(['voiesByStationId', variables.stationId]);
      queryClient.invalidateQueries(['voiesByStationInitiales', variables.stationInitiales]);
    }
  });
};

export const useAddVoieTor = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: async (newVoieTor) => {
      const response = await fetch(`${BASE_URL}/Voies/tor`, {
        method: 'POST',
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: 'same-origin',
        body: JSON.stringify(newVoieTor),
      });
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Erreur lors de l\'ajout de la voie TOR');
      }
      return response.json();
    },
    onSuccess: (data, variables) => {
      queryClient.invalidateQueries(['voiesByStationId', variables.stationId]);
      queryClient.invalidateQueries(['voiesByStationInitiales', variables.stationInitiales]);
    }
  });
};

export const useUpdateVoie = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: async (updatedVoie) => {
      const response = await fetch(`${BASE_URL}/Voies/${updatedVoie.id}`, {
        method: 'PUT',
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: 'same-origin',
        body: JSON.stringify(updatedVoie),
      });
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || 'Erreur lors de la mise à jour de la voie');
      }
      return response.json();
    },
    onSuccess: (data, variables) => {
      queryClient.invalidateQueries(['voiesByStationId', variables.stationId]);
      queryClient.invalidateQueries(['voiesByStationInitiales', variables.stationInitiales]);
    }
  });
};

export const useVoiesByStationId = (stationId) => {
  const { user } = useAuth();
  return useQuery({
    queryKey: ['voiesByStationId', stationId],
    queryFn: async () => {
      if (!stationId) {
        return null;
      }
      const response = await fetch(`${BASE_URL}/Voies/ByStationId/${stationId}`, {
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: 'same-origin',
      });
      if (!response.ok) {
        throw new Error('Erreur lors de la récupération des voies');
      }
      return response.json();
    },
    select: (data) => {
      if (!user || !data) return data;
      return {
        ...data,
        voiesTelemesurees: data.voiesTelemesurees.filter(voie => 
          userHasAccessToVoie(user.profilCode, voie.abonnements)
        )
      };
    },
    enabled: !!stationId,
    refetchOnWindowFocus: false,
    retry: false,
  });
};

export const useUpdateAbonnementsVoie = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ voieId, abonnements }) => {
      const response = await fetch(`${BASE_URL}/Voies/${voieId}/abonnements`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: "same-origin",
        body: JSON.stringify(abonnements),
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(
          errorData.message || "Erreur lors de la mise à jour des abonnements de la voie"
        );
      }

      return response.json();
    },
    onSuccess: (data, variables) => {
      queryClient.invalidateQueries(["voiesByStationId", variables.stationId]);
    },
  });
};