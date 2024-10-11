import React, { useState, useMemo, useEffect } from "react";
import StationSelectorVisu from "@/components/StationBandeauSelection/StationSelectorVisu";
import { useStations } from "@/services/visualisationAPI";

const StationSelectorContainerVisu = ({
  onStationSelect,
  reseau,
  multiSelect = false,
}) => {
  const [selectedStations, setSelectedStations] = useState([]);
  const { data: stations, isLoading, error } = useStations(reseau);

  const filteredStations = useMemo(() => {
    if (!stations) return [];
    return stations.filter((station) => station.reseau === reseau);
  }, [stations, reseau]);

  const handleStationSelect = (station) => {
    setSelectedStations((prevSelectedStations) => {
      if (multiSelect) {
        const isAlreadySelected = prevSelectedStations.some(
          (s) => s.initiales === station.initiales
        );
        if (isAlreadySelected) {
          return prevSelectedStations.filter(
            (s) => s.initiales !== station.initiales
          );
        } else {
          return [...prevSelectedStations, station];
        }
      } else {
        return [station];
      }
    });
  };

  useEffect(() => {
    onStationSelect(selectedStations);
  }, [selectedStations, onStationSelect]);

  if (isLoading) return <div>Chargement des stations...</div>;
  if (error) return <div>Erreur: {error.message}</div>;

  return (
    <StationSelectorVisu
      stations={filteredStations}
      selectedStations={selectedStations}
      onSelect={handleStationSelect}
    />
  );
};

export default StationSelectorContainerVisu;
