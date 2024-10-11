import React, { useState, useMemo } from 'react';
import StationSelector from "@/components/StationBandeauSelection/StationSelector";
import { useStation } from '@/services/stationsAPI';

const StationSelectorContainer = ({ onStationSelect, reseau }) => {
  const [selectedStation, setSelectedStation] = useState(null);
  const { data: stations, isLoading, error } = useStation(reseau);

  const activeStations = useMemo(() => {
    if (!stations) return [];
    return stations.filter(station => station.actif && station.reseau === reseau);
  }, [stations, reseau]);

  const handleStationSelect = (station) => {
    setSelectedStation(station.initiales);
    onStationSelect(station);
  };

  if (isLoading) return <div>Chargement des stations...</div>;
  if (error) return <div>Erreur: {error.message}</div>;

  return (
    <StationSelector
      stations={activeStations}
      selectedStation={selectedStation}
      onSelect={handleStationSelect}
    />
  );
};

export default StationSelectorContainer;