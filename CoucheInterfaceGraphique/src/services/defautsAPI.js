import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getAuthHeaders } from "@/utils/authUtils";
import {
  userHasAccessToStation,
  userHasAccessToVoie,
} from "@/utils/abonnementsUtils";
import { useAuth } from "@/contexts/AuthContext";

const BASE_URL = "/api";

export const useDefautsActifsCapteurUsage = () => {
  const { user } = useAuth();
  return useQuery({
    queryKey: ["defautsActifs"],
    queryFn: async () => {
      const response = await fetch(`${BASE_URL}/Defauts/CapteurUsage`, {
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
    select: (data) => {
      if (!user) {
        console.warn('User is undefined in useDefautsActifsCapteurUsage');
        return data;
      }

      return data.filter((defaut) => {
        const hasStationAccess = userHasAccessToStation(
          user.profilCode,
          defaut.stationAbonnements
        );
        const hasVoieAccess = userHasAccessToVoie(
          user.profilCode,
          defaut.voieAbonnements
        );

        return hasStationAccess && hasVoieAccess;
      });
    },
  });
};

export const useDefautsActifsCapteurObs = () => {
  const { user } = useAuth();
  return useQuery({
    queryKey: ["defautsActifsObs"],
    queryFn: async () => {
      const response = await fetch(`${BASE_URL}/Defauts/CapteurObservation`, {
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
    select: (data) => {
      if (!user) return data;

      return data.filter((defaut) => {
        const hasStationAccess = userHasAccessToStation(
          user.profilCode,
          defaut.stationAbonnements
        );
        const hasVoieAccess = userHasAccessToVoie(
          user.profilCode,
          defaut.voieAbonnements
        );

        return hasStationAccess && hasVoieAccess;
      });
    },
  });
};

export const useDefautsActifsEtatUsage = () => {
  return useQuery({
    queryKey: ["defautsActifsEtatUsage"],
    queryFn: async () => {
      const response = await fetch(`${BASE_URL}/Defauts/EtatUsage`, {
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

export const useDefautsActifsEtatObs = () => {
  return useQuery({
    queryKey: ["defautsActifsEtatObservation"],
    queryFn: async () => {
      const response = await fetch(`${BASE_URL}/Defauts/EtatObservation`, {
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

export const useDefautsActifsPingUsage = () => {
  return useQuery({
    queryKey: ["defautsActifsPingUsage"],
    queryFn: async () => {
      const response = await fetch(`${BASE_URL}/Defauts/PingUsage`, {
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

export const useDefautsActifsPingObs = () => {
  return useQuery({
    queryKey: ["defautsActifsPingObservation"],
    queryFn: async () => {
      const response = await fetch(`${BASE_URL}/Defauts/PingObservation`, {
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

export const useDefautsActifsAutreUsage = () => {
  return useQuery({
    queryKey: ["defautsActifsAutreUsage"],
    queryFn: async () => {
      const response = await fetch(`${BASE_URL}/Defauts/AutreUsage`, {
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

export const useDefautsActifsAutreObs = () => {
  return useQuery({
    queryKey: ["defautsActifsAutreObservation"],
    queryFn: async () => {
      const response = await fetch(`${BASE_URL}/Defauts/AutreObservation`, {
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

export const useUpdateDefautActif = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, commentaire, utilisateur }) => {
      const response = await fetch(`${BASE_URL}/Defauts/${id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: "same-origin",
        body: JSON.stringify({ commentaire, utilisateur }),
      });
      if (!response.ok) {
        throw new Error("Erreur lors de la mise à jour");
      }
    },
    onSuccess: () => {
      queryClient.invalidateQueries(["defautsActifs"]);
    },
  });
};

export const useDefautsCapteurLast24Hours = () => {
  const { user } = useAuth();
  return useQuery({
    queryKey: ["defautsCapteurLast24Hours"],
    queryFn: async () => {
      const response = await fetch(
        `${BASE_URL}/Battements/CapteurLast24Hours`,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            ...getAuthHeaders(),
          },
          credentials: "same-origin",
        }
      );
      if (!response.ok) {
        throw new Error("Erreur " + response.statusText);
      }
      return response.json();
    },
    select: (data) => {
      if (!user) return data;

      return data.filter((defaut) => {
        const hasStationAccess = userHasAccessToStation(
          user.profilCode,
          defaut.stationAbonnements
        );
        const hasVoieAccess = userHasAccessToVoie(
          user.profilCode,
          defaut.voieAbonnements
        );

        return hasStationAccess && hasVoieAccess;
      });
    },
  });
};
