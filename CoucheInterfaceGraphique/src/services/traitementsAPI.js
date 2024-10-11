import { useQuery } from "@tanstack/react-query";
import { getAuthHeaders } from "@/utils/authUtils";

const BASE_URL = "/api/traitements";

export const useVoiesTraitementsByStations = (stationInitiales) => {
  return useQuery({
    queryKey: ["voiesTraitements", stationInitiales],
    queryFn: async () => {
      const response = await fetch(`${BASE_URL}/voies?stationInitiales=${stationInitiales.join(',')}`, {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          ...getAuthHeaders(),
        },
        credentials: "same-origin",
      });
      if (!response.ok) {
        throw new Error("Erreur lors de la récupération des voies avec traitements");
      }
      return response.json();
    },
    enabled: !!stationInitiales && stationInitiales.length > 0,
  });
};