import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { getAuthHeaders } from "@/utils/authUtils";
import { userHasAccessToStation } from "@/utils/abonnementsUtils";
import { useAuth } from "@/contexts/AuthContext";

const BASE_URL = "/api";

export const useStation = (reseau) => {
  const { user } = useAuth();
  return useQuery({
    queryKey: ["stationsWithEnregistreur", reseau],
    queryFn: async () => {
      const response = await fetch(
        `${BASE_URL}/Stations/stationsEnrg?reseau=${reseau}`,
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
          "Erreur lors de la récupération des stations avec enregistreurs"
        );
      }
      return response.json();
    },
    select: (data) => {
      if (!user) return data;
      return data.filter((station) =>
        userHasAccessToStation(user.profilCode, station.abonnements)
      );
    },
  });
};

export const useAddStationWithEnregistreur = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (newStationWithEnregistreur) => {
      try {
        const response = await fetch(
          `${BASE_URL}/Stations/AddStationWithEnrg`,
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
              ...getAuthHeaders(),
            },
            credentials: "same-origin",
            body: JSON.stringify(newStationWithEnregistreur),
          }
        );

        if (!response.ok) {
          const errorData = await response.json();
          console.error("Erreur détaillée du serveur:", errorData);
          throw new Error(
            errorData.message ||
              `Erreur ${response.status}: ${response.statusText}`
          );
        }

        const data = await response.json();
        return data;
      } catch (error) {
        console.error("Erreur lors de l'ajout de la station:", error);
        throw error;
      }
    },
    onSuccess: (data, variables) => {
      queryClient.invalidateQueries([
        "stationsWithEnregistreur",
        variables.reseau,
      ]);
    },
  });
};

export const useUpdateAbonnements = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ stationId, abonnements }) => {
      const response = await fetch(
        `${BASE_URL}/Stations/${stationId}/abonnements`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            ...getAuthHeaders(),
          },
          credentials: "same-origin",
          body: JSON.stringify(abonnements), // Envoyez directement la valeur encodée
        }
      );

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(
          errorData.message || "Erreur lors de la mise à jour des abonnements"
        );
      }

      return response.json();
    },
    onSuccess: () => {
      queryClient.invalidateQueries(["stationsWithEnregistreur"]);
    },
  });
};

export const useUpdateStation = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (updatedData) => {
      const { station, enregistreur } = updatedData;
      const response = await fetch(`${BASE_URL}/Stations/${station.id}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: "same-origin",
        body: JSON.stringify({
          station: {
            id: station.id,
            initiales: station.initiales,
            nom: station.nom,
            numero: station.numero,
            bassinVersant: station.bassinVersant,
            actif: station.actif,
            enregistreurId: station.enregistreurId,
            reseau: station.reseau,
            abonnements: station.abonnements,
            preselections: station.preselections,
          },
          enregistreur: {
            id: enregistreur.id,
            adresseIp: enregistreur.adresseIp,
            liaison: enregistreur.liaison,
            version: enregistreur.version,
            dateMaj: enregistreur.dateMaj,
            dernierTransfert: enregistreur.dernierTransfert,
            dernierAppel: enregistreur.dernierAppel,
            dernierEnregistrement: enregistreur.dernierEnregistrement,
            pourcentageMemoire: enregistreur.pourcentageMemoire,
            typeHeure: enregistreur.typeHeure,
          },
        }),
      });
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(
          errorData.message || "Erreur lors de la mise à jour de la station"
        );
      }
      const data = await response.json();
      return data;
    },
    onSuccess: (data, variables) => {
      queryClient.invalidateQueries([
        "stationsWithEnregistreur",
        variables.station.reseau,
      ]);
    },
  });
};

export const useAvailableStationNumbers = () => {
  return useQuery({
    queryKey: ["availableStationNumbers"],
    queryFn: async () => {
      const response = await fetch(`${BASE_URL}/Stations/AvailableNumbers`, {
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: "same-origin",
      });
      if (!response.ok) {
        throw new Error(
          "Erreur lors de la récupération des numéros de station disponibles"
        );
      }
      return response.json();
    },
  });
};

export const useCreateStationFromModel = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ modelStationId, newStation }) => {
      const response = await fetch(`${BASE_URL}/Stations/CreateFromModel`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: "same-origin",
        body: JSON.stringify({ modelStationId, newStation }),
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(
          errorData.message ||
            `Erreur ${response.status}: ${response.statusText}`
        );
      }

      return response.json();
    },
    onSuccess: (data, variables) => {
      queryClient.invalidateQueries([
        "stationsWithEnregistreur",
        variables.newStation.reseau,
      ]);
    },
  });
};

export const useStationDetails = () => {
  const { user } = useAuth();
  return useQuery({
    queryKey: ["stationDetails"],
    queryFn: async () => {
      try {
        const response = await fetch(`${BASE_URL}/Stations/details`, {
          headers: {
            "Content-Type": "application/json",
            ...getAuthHeaders(),
          },
          credentials: "same-origin",
        });
        if (!response.ok) {
          throw new Error(
            "Erreur lors de la récupération des détails des stations"
          );
        }
        const data = await response.json();
        return data;
      } catch (error) {
        console.error("Erreur dans useStationDetails:", error);
        throw error;
      }
    },
    select: (data) => {
      if (!user) return data;
      return data.filter((station) =>
        userHasAccessToStation(user.profilCode, station.abonnements)
      );
    },
  });
};

export const useStationDetailsObs = () => {
  const { user } = useAuth();
  return useQuery({
    queryKey: ["stationDetails"],
    queryFn: async () => {
      try {
        const response = await fetch(`${BASE_URL}/Stations/detailsObs`, {
          headers: {
            "Content-Type": "application/json",
            ...getAuthHeaders(),
          },
          credentials: "same-origin",
        });
        if (!response.ok) {
          throw new Error(
            "Erreur lors de la récupération des détails des stations"
          );
        }
        const data = await response.json();
        return data;
      } catch (error) {
        console.error("Erreur dans useStationDetails:", error);
        throw error;
      }
    },
    select: (data) => {
      if (!user) return data;
      return data.filter((station) =>
        userHasAccessToStation(user.profilCode, station.abonnements)
      );
    },
  });
};

export const useUpdatePreselections = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ stationId, preselections }) => {
      const response = await fetch(
        `${BASE_URL}/Stations/${stationId}/preselections`,
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            ...getAuthHeaders(),
          },
          credentials: "same-origin",
          body: JSON.stringify(preselections),
        }
      );

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(
          errorData.message || "Erreur lors de la mise à jour des présélections"
        );
      }

      return response.json();
    },
    onSuccess: () => {
      queryClient.invalidateQueries(["stationsWithEnregistreur"]);
    },
  });
};

export const useAppelerStations = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (stationIds) => {
      console.log("Appel des stations avec les IDs:", stationIds);
      const response = await fetch(`${BASE_URL}/Stations/AppelerStation`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          ...getAuthHeaders(),
        },
        credentials: 'same-origin',
        body: JSON.stringify(stationIds),
      });

      const responseData = await response.json();
      console.log("Données de la réponse:", responseData);

      if (!response.ok) {
        throw new Error(JSON.stringify({
          message: responseData.message || "Une erreur inconnue est survenue",
          details: responseData.details || ""
        }));
      }
      
      return responseData;

    },
    onSuccess: (data) => {
      console.log("Données reçues après l'appel des stations:", data);
      queryClient.invalidateQueries(['stationsWithEnregistreur']);
    },
    onError: (error) => {
      console.error("Erreur lors de l'appel des stations:", error.message);
      throw error;
    },
  });
};