import { useCallback } from 'react';
import { format } from 'date-fns';
import { useHistoricalDataMultiple } from '@/services/visualisationAPI';

export const useDataFetching = (selectedStations, dateRange, selectedTors, selectedAnas) => {
  const getHistoricalDataMultiple = useHistoricalDataMultiple();

  return useCallback(async () => {
    if (selectedTors.length === 0 && selectedAnas.length === 0) {
      throw new Error("Veuillez sélectionner au moins une voie ANA ou TOR");
    }

    if (!selectedStations || (Array.isArray(selectedStations) && selectedStations.length === 0)) {
      throw new Error("Veuillez sélectionner au moins une station");
    }

    const startDateTime = format(dateRange[0], "yyyy-MM-dd HH:mm:ss");
    const endDateTime = format(dateRange[1], "yyyy-MM-dd HH:mm:ss");

    try {
      const selectedVoies = [...selectedTors, ...selectedAnas];
      // console.log("Fetching data for stations:", selectedStations);
      // console.log("Fetching data for voies:", selectedVoies);
      
      const historicalData = await getHistoricalDataMultiple(
        selectedStations,
        selectedVoies,
        startDateTime,
        endDateTime
      );

      // console.log("Received data count:", historicalData.length);
      return historicalData;
    } catch (error) {
      console.error("Error fetching historical data:", error);
      const errorMessage = error.response?.data?.details || error.message || "Une erreur inconnue s'est produite";
      throw new Error(`Erreur lors de la récupération des données historiques: ${errorMessage}`);
    }
  }, [selectedStations, dateRange, selectedTors, selectedAnas, getHistoricalDataMultiple]);
};