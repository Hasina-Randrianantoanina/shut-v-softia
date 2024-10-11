import { useQuery, useQueryClient } from "@tanstack/react-query";
import { useStation } from "@/services/stationsAPI";
import { useEnregistreurs } from "@/services/enregistreursAPI";
import { getAuthHeaders, setToken } from "@/utils/authUtils";

const BASE_URL = "/api";

export const useStations = () => {
  const { data: stationsFromStationsAPI, isLoading: isLoadingStationsAPI } =
    useStation();
  const { data: enregistreurs, isLoading: isLoadingEnregistreurs } =
    useEnregistreurs();

  return useQuery({
    queryKey: ["visualisationStations"],
    queryFn: async () => {
      const response = await fetch(`${BASE_URL}/Visualisation/Stations`);
      if (!response.ok) {
        throw new Error("Erreur lors de la récupération des stations");
      }
      const visuStations = await response.json();

      if (!stationsFromStationsAPI || !enregistreurs) {
        return visuStations.map((station) => ({
          initiales: station,
          nom: station,
          enregistreurVersion: "",
          liaison: "",
          reseau: "",
        }));
      }

      const detailedStations = visuStations.map((visuStation) => {
        const matchingStation = stationsFromStationsAPI.find(
          (s) => s.initiales.toLowerCase() === visuStation.toLowerCase()
        );
        const matchingEnregistreur = enregistreurs.find(
          (e) => e.id === matchingStation?.enregistreurId
        );

        return {
          ...matchingStation,
          initiales: visuStation,
          nom: matchingStation?.nom || visuStation,
          enregistreurVersion: matchingEnregistreur?.version || "",
          liaison: matchingEnregistreur?.liaison || "",
          reseau: matchingStation?.reseau || "",
        };
      });

      return detailedStations;
    },
    enabled: !isLoadingStationsAPI && !isLoadingEnregistreurs,
  });
};

export const useVoies = (selectedStations) => {
  return useQuery({
    queryKey: ["voies", selectedStations],
    queryFn: async () => {
      if (!selectedStations || selectedStations.length === 0) {
        return [];
      }
      try {
        const queryParams = selectedStations
          .map((station) => `stations=${encodeURIComponent(station)}`)
          .join("&");
        const response = await fetch(
          `${BASE_URL}/Visualisation/Voies?${queryParams}`,
          {
            headers: {
              "Content-Type": "application/json",
              ...getAuthHeaders(),
            },
            credentials: "same-origin",
          }
        );
        if (!response.ok) {
          const errorText = await response.text();
          throw new Error(`Erreur réseau (${response.status}): ${errorText}`);
        }
        const data = await response.json();
        return Object.entries(data).map(([station, voies]) => ({
          name: station,
          analogVoies: voies.filter(
            (voie) =>
              !voie.toLowerCase().includes("etor") &&
              !voie.toLowerCase().includes("stor")
          ),
          torVoies: voies.filter(
            (voie) =>
              voie.toLowerCase().includes("etor") ||
              voie.toLowerCase().includes("stor")
          ),
        }));
      } catch (error) {
        console.error(`Erreur lors de la récupération des voies:`, error);
        throw error;
      }
    },
    enabled: !!selectedStations && selectedStations.length > 0,
  });
};

export const useHistoricalDataMultiple = () => {
  const queryClient = useQueryClient();

  return async (
    selectedStations,
    selectedVoies,
    startDateTime,
    endDateTime
  ) => {
    if (
      !selectedStations ||
      (Array.isArray(selectedStations) && selectedStations.length === 0) ||
      !selectedVoies ||
      selectedVoies.length === 0
    ) {
      return [];
    }

    const stationsArray = Array.isArray(selectedStations)
      ? selectedStations
      : [selectedStations];

    const queryKey = [
      "historicalDataMultiple",
      stationsArray,
      selectedVoies,
      startDateTime,
      endDateTime,
    ];

    return queryClient.fetchQuery({
      queryKey,
      queryFn: async () => {
        const url = new URL(
          `${window.location.origin}${BASE_URL}/Visualisation/HistoriqueMultipleVoies`
        );

        // Regrouper les voies par station
        const voiesByStation = selectedVoies.reduce((acc, voie) => {
          if (!acc[voie.station]) {
            acc[voie.station] = [];
          }
          acc[voie.station].push(voie.libelle);
          return acc;
        }, {});

        // Ajouter chaque station une seule fois avec toutes ses voies
        Object.entries(voiesByStation).forEach(([station, voies]) => {
          url.searchParams.append("stations", station);
          voies.forEach((voie) =>
            url.searchParams.append("voies", `${station}:${voie}`)
          );
        });

        url.searchParams.append("start", startDateTime);
        url.searchParams.append("end", endDateTime);

        const response = await fetch(url, {
          headers: {
            "Content-Type": "application/json",
            ...getAuthHeaders(),
          },
          credentials: "same-origin",
        });
        if (!response.ok) {
          const errorText = await response.text();
          throw new Error(
            `Erreur lors de la récupération des données historiques: ${errorText}`
          );
        }
        return response.json();
      },
    });
  };
};
